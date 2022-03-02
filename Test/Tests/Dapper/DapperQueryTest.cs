using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Autofac;
using Dapper;
using Data.Cqrs.Test.Dapper;
using Kovi.Data.Cqrs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Data.Cqrs.Test
{
	[TestClass]
	public class DapperQueryTest
		: BaseAutofacTest
	{
		protected override Module AutofacModule => new DapperAutofacModule();

		[TestMethod]
		public void QueryHandler_ForDapperAllBookQriteria_ReturnsData()
		{
			// Arrange
			var qh = Container.Resolve<IQueryHandler<DapperAllFlatBookQriteria, IEnumerable<BookDto>>>();
			var qrit = new DapperAllFlatBookQriteria { };
			// Act
			var actual = qh.Handle(qrit);
			// Assert
			Assert.IsTrue(actual.Any());
			Assert.IsTrue(actual.All(b => b.Name != null));
		}

		[TestMethod]
		public void QueryHandler_ForDapperBookMapQriteria_ReturnsData()
		{
			// Arrange
			var qh = Container.Resolve<IQueryHandler<DapperAllBookQriteria, IEnumerable<BookDto>>>();
			var qrit = new DapperAllBookQriteria { };
			// Act
			var actual = qh.Handle(qrit);
			// Assert
			Assert.IsTrue(actual.Any());
			Assert.IsTrue(actual.All(b => b.Name != null));
			// проверка уникальности Book.Id
			Assert.IsFalse(actual.GroupBy(b => b.Id).Where(x=>x.Count()>1).Any());
			foreach (var item in actual)
			{
				Console.WriteLine(item);
			}
		}

		[TestMethod]
		public void ConnectionFactory_Creates_DbConnection()
		{
			using (var dbConn = Container.Resolve<IConnectionFactory>().Create())
			{
				Assert.IsInstanceOfType(dbConn, typeof(IDbConnection));
				try
				{
					dbConn.Open();
				}
				catch (Exception ex)
				{
					Assert.Fail($"Connection is not open. {ex.Message}");
				}
			}
		}

		[TestMethod]
		public void DapperQuery_On_DbConnection()
		{
			const string sql = 
@"
select count(id) from [Book];
select *
from [Book] b
left join [BookAuthor] ba on b.Id=ba.BookId
left join [Author] a on ba.AuthorId=a.Id;";

			var books = new Dictionary<int, BookDto>();

			using (var dbConn = Container.Resolve<IConnectionFactory>().Create())
			{
				try
				{
					dbConn.Open();
					using (SqlMapper.GridReader dapper =dbConn.QueryMultiple(sql))
					{
						var cnt = dapper.ReadFirst<int>();
						var rec = dapper.Read<BookDto>();
					}
				}
				catch (Exception ex)
				{
					Assert.Fail($"Connection is not open. {ex.Message}");
				}

				//foreach (var item in actual1)
				//{
				//	Console.WriteLine(item);
				//}
			}
		}


		[TestMethod]
		public void DapperQueryHandler_Returns_Data()
		{
			var qh = Container.Resolve<IQueryHandler<DapperAllBookQriteria, IEnumerable<BookDto>>>();
			var qrit = new DapperAllBookQriteria();

			var actual = qh.Handle(qrit);
			foreach (var item in actual)
			{
				Console.WriteLine(item.Name);
			}
			Assert.IsTrue(actual.Any());
		}

		//[TestMethod]
		//public void DapperQueryHandler_WithPredefinedConnection_Returns_Data()
		//{
		//	var qh = Container.Resolve<IQueryHandler<DapperConnectionBookQriteria, IEnumerable<BookDto>>>();
		//	var qrit = new DapperConnectionBookQriteria();

		//	var actual = qh.Handle(qrit);
		//	foreach (var item in actual)
		//	{
		//		Console.WriteLine(item.Name);
		//	}
		//	Assert.IsTrue(actual.Any());
		//}

		//[TestMethod]
		//public void DapperBookById_Returns_Book()
		//{
		//	var qh = Container.Resolve<IQueryHandler<DapperBookByIdQriteria, BookDto>>();
		//	var qrit = new DapperBookByIdQriteria() { Id = 2 };

		//	var actual = qh.Handle(qrit);

		//	Assert.AreEqual(qrit.Id, actual.Id);
		//}
	}
}