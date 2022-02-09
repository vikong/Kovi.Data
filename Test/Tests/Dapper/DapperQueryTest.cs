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
select *
from [Book] b
left join [BookAuthor] ba on b.Id=ba.BookId
left join [Author] a on ba.AuthorId=a.Id";

			var books = new Dictionary<int, BookDto>();

			using (var dbConn = Container.Resolve<IConnectionFactory>().Create())
			{
				try
				{
					dbConn.Open();
				}
				catch (Exception ex)
				{
					Assert.Fail($"Connection is not open. {ex.Message}");
				}

				BookDto mapFunc(Book b, Author a)
				{
					BookDto bookEntry;
					if (!books.TryGetValue(b.Id, out bookEntry))
					{
						bookEntry = new BookDto
						{
							Id = b.Id,
							Name = b.Name,
							Raiting = b.Raiting,
						};
						books.Add(bookEntry.Id, bookEntry);
					}
					var author = new AuthorDto { Id = a.Id, Name = a.Name };
					bookEntry.BookAuthors.Add(new BookAuthorDto { Book = bookEntry, Author = author });
					return bookEntry;
				};
				
				var actual = dbConn.Query<Book, Author, BookDto>(sql,
					map: mapFunc,
					splitOn: "AuthorId")
					.Distinct()
					.ToList();

				var books1 = new Dictionary<int, BookDto>();

				BookDto mapFunc1(Object[] par)
				{
					Book b = par[0] as Book;
					Author a = par[1] as Author;
					BookDto bookEntry;
					if (!books1.TryGetValue(b.Id, out bookEntry))
					{
						bookEntry = new BookDto
						{
							Id = b.Id,
							Name = b.Name,
							Raiting = b.Raiting,
						};
						books1.Add(bookEntry.Id, bookEntry);
					}
					var author = new AuthorDto { Id = a.Id, Name = a.Name };
					bookEntry.BookAuthors.Add(new BookAuthorDto { Book = bookEntry, Author = author });
					return bookEntry;
				};
				var actual1 = dbConn.Query<BookDto>(sql, 
					new Type[] { typeof(Book), typeof(Author) },
					map: mapFunc1,
					splitOn: "AuthorId",
					param: null)
					.Distinct()
					.ToList();

				//var actual = dbConn.Query<Book,Author,BookDto>(sql, 
				//	map:(b,a)=>{
				//		BookDto bookEntry;
				//		if (!books.TryGetValue(b.Id, out bookEntry))
				//		{ 
				//			bookEntry = new BookDto
				//			{
				//				Id = b.Id,
				//				Name = b.Name,
				//				Raiting = b.Raiting,
				//			};
				//			books.Add(bookEntry.Id, bookEntry);
				//		}
				//		var author = new AuthorDto { Id = a.Id, Name = a.Name };
				//		bookEntry.BookAuthors.Add(new BookAuthorDto { Book=bookEntry, Author = author });
				//		return bookEntry;
				//	}, 
				//	splitOn: "AuthorId")
				//	.Distinct()
				//	.ToList();
				foreach (var item in actual1)
				{
					Console.WriteLine(item);
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