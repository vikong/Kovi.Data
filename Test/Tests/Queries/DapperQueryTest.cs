using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Autofac;
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

		[TestMethod]
		public void DapperQueryHandler_WithPredefinedConnection_Returns_Data()
		{
			var qh = Container.Resolve<IQueryHandler<DapperConnectionBookQriteria, IEnumerable<BookDto>>>();
			var qrit = new DapperConnectionBookQriteria();

			var actual = qh.Handle(qrit);
			foreach (var item in actual)
			{
				Console.WriteLine(item.Name);
			}
			Assert.IsTrue(actual.Any());
		}

		[TestMethod]
		public void DapperBookById_Returns_Book()
		{
			var qh = Container.Resolve<IQueryHandler<DapperBookByIdQriteria, BookDto>>();
			var qrit = new DapperBookByIdQriteria() { Id = 2 };

			var actual = qh.Handle(qrit);

			Assert.AreEqual(qrit.Id, actual.Id);
		}
	}
}