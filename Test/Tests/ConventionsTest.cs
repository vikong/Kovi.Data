using System;
using System.Diagnostics;
using System.Linq;
using Data.Cqrs.Test.EF;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kovi.LinqExtensions;
using System.Linq.Expressions;
using System.Collections.Generic;
using Kovi.Data.Cqrs.Infrastructure;
using Kovi.Data.Cqrs.Linq;
using Kovi.Data.Cqrs;
using Moq;

namespace Data.Cqrs.Test
{
	[TestClass]
	public class TestConventions
	{
		private readonly List<Author> _authors = new List<Author>()
		{
			new Author(1, "bbb"),
			new Author(2, "aaa"),
			new Author(3, "ddd"),
			new Author(4, "ccc")
		};

		private IQueryable<Author> Authors => _authors.AsQueryable();

		[TestMethod]
		public void TestFilterConvention()
		{
			Expression<Func<Author, Boolean>> expression = (a) => a.Name.StartsWith("a");
			var func = expression.Compile();

			Assert.IsTrue(func.Invoke(new Author(1, "araf")));
			Assert.IsFalse(func.Invoke(new Author(1, "dff")));

			var filterExpr = ConventionBuilder<Author>.FilterExpression(new AuthorDto{ Name = "a" });
			func = filterExpr.Compile();
			Assert.IsTrue(func.Invoke(new Author(1, "araf")));
			Assert.IsFalse(func.Invoke(new Author(1, "dff")));

			filterExpr = ConventionBuilder<Author>.FilterExpression(new BookOrAuthorQriteria { Name = "a" });
			func = filterExpr.Compile();
			Assert.IsTrue(func.Invoke(new Author(1, "araf")));
			Assert.IsFalse(func.Invoke(new Author(1, "dff")));

			//AuthorDto author = new AuthorDto { Name = "a" };
			//var result = Conventions<Author>.FilterExpression(author);

			//Assert.AreEqual(2, Authors.Where(result).First().Id);
		}
		[TestMethod]
		public void IdConventionQuery_Returns_EntityWithId()
		{
		}

		[TestMethod]
		public void IdConvention_Returns_EntityWithId()
		{
			using (var uowStub = Stub.CreateMemoryUoW(typeof(Author)).AddAuthors(5))
			{
				var qrit = new IdQriteria<Int32> { Id = 3 };

				var e = new ConventionIdLinqSpec<Author>();
				//var e = ConventionFactory<AuthorDto>.IdSpec();
				var actual = e.Query(uowStub.Linq, qrit)
					.Cast<Author>()
					.Single();

				Assert.AreEqual(qrit.Id, actual.Id);
			}
		}


		//public void IdConvention_Returns_EntityWithId()
		//{
		//	var uowStub = Stub.CreateMemoryUoW(typeof(Author));
		//	uowStub.AddAuthors(5);
		//	var qrit = new IdQriteria<Int32> { Id=3 };
		//	var e = new ConventionIdLinqSpec<Author>();

		//	var actual = e.Query(uowStub.Linq,qrit)
		//		.Cast<Author>()
		//		.Single();

		//	Assert.AreEqual(qrit.Id, actual.Id);
		//}

		[TestMethod]
		public void IdConventionExpression_Returns_EntityWithId()
		{
			using (var uowStub = Stub.CreateMemoryUoW(typeof(Author)))
			{
				uowStub.AddAuthors(5);
				var qrit = new IdQriteria<Int32> { Id = 3 };
				var expr = ConventionBuilder<Author>
					.IdFilterExpression(qrit);

				var actual = uowStub.Linq.Query<Author>()
					.Where(expr)
					.Cast<Author>()
					.Single();

				Assert.AreEqual(qrit.Id, actual.Id);
			}
		}

		[TestMethod]
		public void IdStringConventionExpression_Returns_EntityWithId()
		{
			using (var uow = Stub.CreateMemoryUoW(typeof(BookAuthor)))
			{
				uow.Add(new BookAuthor(new Book(1, "Book 1"), new Author(1, "Author 1")));
				uow.Add(new BookAuthor(new Book(2, "Book 2"), new Author(1, "Author 1")));
				uow.Add(new BookAuthor(new Book(3, "Book 2"), new Author(2, "Author 2")));

				var qrit = new IdQriteria<String> { Id = "2-1" };
				var expr = ConventionBuilder<BookAuthor>
					.IdFilterExpression(qrit);

				var actual = uow.Linq.Query<BookAuthor>().Where(expr)
					.Cast<BookAuthor>()
					.Single();

				Assert.AreEqual(qrit.Id, actual.Id);
			}
		}

		[TestMethod]
		public void PageSpec_Returns_FilteredByConvention()
		{
			//Arrange
			const int authorCount = 25;
			using (var UoWStub = Stub.CreateMemoryUoW(typeof(Author))
				.Add(
					authorCount,
					(i) => new Author(i, "A-" + Convert.ToString(i).PadLeft(2, '0'))
				))
			{
				var linqProvider = UoWStub.Linq;

				var qrit = new PageQriteria<AuthorDto> { PageNo = 3, PageSize = 5, Subject = new AuthorDto { Name = "A-01" } };

				var expected = linqProvider.Query<Author>()
					.Where(a => a.Name.StartsWith(qrit.Subject.Name))
					.OrderBy(a => a.Id)
					.ToArray();

				var spec = new ConventionPagedSpec<Author, AuthorDto>();

				var stopWatch = new Stopwatch();

				//Act
				stopWatch.Start();
				var actual = spec.Query(linqProvider, qrit)
					.Cast<Author>()
					.ToArray();
				stopWatch.Stop();

				//Assert
				CollectionAssert.AreEqual(expected, actual);
				Assert.IsTrue(stopWatch.ElapsedMilliseconds < 100);
			}
		}

		[TestMethod]
		public void PageSpec_Returns_FilteredAndSortedByConvention()
		{
			//Arrange
			const int authorCount = 25;
			using (var UoWStub = Stub.CreateMemoryUoW(typeof(Author))
				.Add(
					authorCount,
					(i) => new Author(i, "A" + (i % 2 == 0 ? "=" : "-") + Convert.ToString(i).PadLeft(2, '0'))
				))
			{
				var linqProvider = UoWStub.Linq;

				var qrit = new PageQriteria<AuthorDto>
				{
					PageNo = 3,
					PageSize = 5,
					OrderBy = "Name",
					Subject = new AuthorDto { Name = "A" }
				};

				var expected = linqProvider.Query<Author>()
					.Where(a => a.Name.StartsWith(qrit.Subject.Name))
					.OrderBy(a => a.Name)
					.ToArray();

				var spec = new ConventionPagedSpec<Author, AuthorDto>();

				var stopWatch = new Stopwatch();

				//Act
				stopWatch.Start();
				var actual = spec.Query(linqProvider, qrit)
					.Cast<Author>()
					.ToArray();
				stopWatch.Stop();

				//Assert
				CollectionAssert.AreEqual(expected, actual);
				Assert.IsTrue(stopWatch.ElapsedMilliseconds < 100);
			}
		}

		[TestMethod]
		public void TestSortConvention()
		{
			var autoSortExpr = ConventionBuilder<Author>.SortExpression("Name");
			Expression<Func<Author, Object>> expr = (a) => a.Name;

			Assert.ThrowsException<InvalidOperationException>(() => ConventionBuilder<Author>.SortExpression("Fake"));
			Assert.AreEqual(expr.Compile().Invoke(_authors[0]), autoSortExpr.Compile().Invoke(_authors[0]));
			Assert.AreEqual(2, Authors.OrderBy(autoSortExpr).First().Id);
		}
	}
}

