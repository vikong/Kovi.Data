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
using System.Data.Entity;
using System.Collections;

namespace Data.Cqrs.Test
{
	[TestClass]
	public class UnitTest2
	{
		[TestMethod]
		public void TakePage()
		{
			Author[] authors = {
				new Author(1, "a1"),
				new Author(2, "a2"),
				new Author(3, "a3"),
				new Author(4, "a4"),
				new Author(5, "a5"),
				new Author(6, "a6"),
				new Author(7, "a7"),
				new Author(8, "a8"),
				new Author(9, "a9")
			};

			IQueryable queryableData = authors.AsQueryable().OrderBy(a => a.Id);
			int page = 2, pageSize = 3;
			
			var actual = queryableData.TakePage(page, pageSize).Cast<Author>().ToList();
			var expected = authors.AsQueryable<Author>()
				.OrderBy(a => a.Id)
				.Skip(pageSize*(page-1))
				.Take(pageSize).ToList();

			CollectionAssert.AreEqual(expected, actual);
			


		}


		[TestMethod]
		public void ConventionsTest()
		{
			var idQrit = new IdQriteria{Id=1};
			var authorConv = Conventions<AuthorDto>.IdSpec;
			var authorPageConv = Conventions<AuthorDto>.PageSpec();

			Assert.ThrowsException<InvalidOperationException>(() => Conventions<PublisherDto>.IdSpec);
		}

		[TestMethod]
		public void ContextExperiments()
		{
			//System.Data.Entity.Database.SetInitializer(new BooksInitializer());
			using (BookContext ctx = new BookContext())
			{
				Debug.WriteLine($"Database: {ctx.Database.Connection.ConnectionString}");
				ctx.Database.Log = s => Debug.WriteLine(s);
				ctx.Database.Initialize(true);

				Assert.IsNotNull(ctx.Books);
				Assert.IsTrue(ctx.Books.Any());

				Book newBook = new Book("new book");
				newBook.AddAuthor(1);
				newBook.AddGenre(1);
				foreach (var a in newBook.BookAuthors)
				{
					var entry = ctx.Entry(a);
					Debug.WriteLine($"{a.Author.Id}:{a.ToString()}, {entry.State}");
					entry.State = System.Data.Entity.EntityState.Unchanged;
				}

				ctx.Books.Add(newBook);
				foreach (var e in ctx.ChangeTracker.Entries())
					Debug.WriteLine($"{e.Entity.GetType().Name}: {e.State}");
				ctx.SaveChanges();
			}
		}

		[TestMethod]
		public void ManuallyCreateQuery()
		{
			Author[] authors = {
				new Author(1, "a1"),
				new Author(2, "a2"),
				new Author(3, "a3"),
				new Author(4, "a4")
			};
			IQueryable queryableData = authors.AsQueryable();

			var entityType = typeof(Author);

			ParameterExpression pe = Expression.Parameter(entityType, "a");

			Expression left = Expression.Property(pe, "Id");

			Expression right = Expression.Constant(2);

			Expression myExpr = Expression.Equal(left, right);

			var etype = typeof(Func<,>).MakeGenericType(entityType, typeof(Boolean));

			//var predicate = Expression.Lambda<Func<Author, Boolean>>(myExpr, new ParameterExpression[] { pe });
			var predicate = Expression.Lambda(etype, myExpr, new ParameterExpression[] { pe });

			MethodCallExpression whereCallExpression = Expression.Call(
				typeof(Queryable),
				"Where",
				new Type[] { entityType }, //{ queryableData.ElementType },
				queryableData.Expression,
				predicate);

			IQueryable results = queryableData.Provider.CreateQuery(whereCallExpression);

			foreach (var a in results)
				Console.WriteLine(a);

		}

		[TestMethod]
		public void ManuallyCreateQuery1()
		{
			Author[] authors = {
				new Author(1, "a1"),
				new Author(2, "a2"),
				new Author(3, "a3"),
				new Author(4, "a4")
			};
			IQueryable queryableData = authors.AsQueryable();

			var entityType = typeof(Author);

			//ParameterExpression pe = Expression.Parameter(entityType, "a");

			//Expression left = Expression.Property(pe, "Id");

			//Expression right = Expression.Constant(2);

			//Expression myExpr = Expression.Equal(left, right);

			//var etype = typeof(Func<,>).MakeGenericType(entityType, typeof(Boolean));

			//var predicate = Expression.Lambda<Func<Author, Boolean>>(myExpr, new ParameterExpression[] { pe });
			//var predicate = Expression.Lambda(etype, myExpr, new ParameterExpression[] { pe });

			//MethodCallExpression whereCallExpression = Expression.Call(
			//	typeof(Queryable),
			//	"Where",
			//	new Type[] { entityType }, //{ queryableData.ElementType },
			//	queryableData.Expression,
			//	predicate);

			MethodCallExpression countCallExpr = Expression.Call(
				typeof(Queryable),
				"Count",
				new Type[] { entityType }, 
				queryableData.Expression);

			//IQueryable results = queryableData.Provider.CreateQuery(whereCallExpression);
			var res = queryableData.Provider.Execute<Int32>(countCallExpr);

			Console.WriteLine(res);
			//foreach (var a in results)
			//	Console.WriteLine(a);

		}


	}
}
