using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kovi.LinqExtensions;
using System.Reflection;
using System.Collections.Concurrent;
using System.Threading;
using Kovi.LinqExtensions.Expressions;
using Kovi.LinqExtensions.Specification;
using Kovi.Data.Cqrs.Linq;
using Kovi.Data.Cqrs;
using System.Threading.Tasks;
using Data.Cqrs.Test.EF;

namespace Data.Cqrs.Test
{

	class My<T>
	{
		public static Boolean IsEnum()
			=> typeof(IEnumerable).IsAssignableFrom(typeof(T)) ;
	}

	class StringMy : My<IEnumerable<String>> { }

	[TestClass]
	public class RulesTest
	{
		private readonly List<Book> books;
		private readonly List<Author> authors;
		private readonly List<Genre> genres;

		public RulesTest()
		{
			books = new List<Book>();

			authors = new List<Author>{
				new Author(1,"author 1"),
				new Author(2,"author 2"),
				new Author(3,"author 3"),
				new Author(4,"author 4")
			};

			genres = new List<Genre> {
				new Genre("<не указан>"),
				new Genre("Учебная"),
				new Genre("Фантастика"),
				new Genre("Фэнтези"),
				new Genre("Учебная")
			};

			Book book;
			AddBook(1, "book1", genres[1], authors[1]);

			book=AddBook(2, "book2", genres[2], authors[2]); 
			book.Raiting = 9; book.HasElectronic = true;

			AddBook(3, "book2-1", genres[3], authors[2]);
			AddBook(4, "book2-2", genres[3], authors[2]);
			AddBook(5, "book3&4", genres[4], authors.Where(a => a.Id == 3 || a.Id == 4));
		}

		private Book AddBook(Int32 bookId, String bookName, Genre genre, IEnumerable<Author> authors)
		{
			Book book = new Book(bookId, bookName);
			book.AddAuthors(authors);
			book.AddGenre(genre);
			foreach (Author author in authors)
			{
				author.Books.Add(book);
			}
			books.Add(book);
			return book;
		}

		private Book AddBook(Int32 bookId, String bookName, Genre genre, Author author)
		{
			Book book = new Book(bookId, bookName);
			book.AddAuthor(author);
			book.AddGenre(genre);
			author.Books.Add(book);
			books.Add(book);
			return book;
		}

		[TestMethod]
		public void ExpressionTest()
		{
			// жанр фантастики
			Expression<Func<Genre, bool>> fantasticGenreExpr = 
				(g) => g.Name == "фантастика" || g.Name == "фэнтези";

			System.Diagnostics.Debug.WriteLine($"Rule: {fantasticGenreExpr.ToString()}");

			// книги в жанре фантастики
			Expression<Func<BookGenre, Genre>> convertExpr = 
				(b) => b.Genre;

			var fantasticBookGenre = convertExpr.Predicated(fantasticGenreExpr);
			// то же самое
			var fantasticBookGenre1 = fantasticGenreExpr.With<BookGenre,Genre>((b)=>b.Genre);

			System.Diagnostics.Debug.WriteLine(fantasticBookGenre.ToString());

			var query = 
				books.AsQueryable()
				.Where
				(
					(b)=>
					b.BookGenres.AsQueryable()
					.Any(fantasticBookGenre)
				);

			var res = query.ToList();
			Assert.AreEqual(3, query.Count());

			// авторы, написавшие хотя бы одну книгу в жанре фантастики
			Expression<Func<Author,Boolean>> AuthorOfFantastic = 
				(a) => a.Books.AsQueryable()
				.Any(
						(b)=>b.BookGenres.AsQueryable()
						.Any(fantasticBookGenre)
					);

			System.Diagnostics.Debug.WriteLine($"author: {AuthorOfFantastic.ToString()}");

			var q = authors.AsQueryable()
				.Where(AuthorOfFantastic)
				.ToList();

			Assert.AreEqual(1, q.Count);
		}

		[TestMethod]
		public void RulesFuncTest()
		{
			var notPopularBooks = books.AsQueryable().Where(!Book.Rules.PopularBook);
			foreach (var book in notPopularBooks)
			{
				Assert.IsFalse(Book.Rules.PopularBook.Func(book));
				Assert.IsFalse(Book.Rules.PopularBook.IsSatisfiedBy(book));
				Assert.IsFalse(book.Is(Book.Rules.PopularBook));
			}

		}

		[TestMethod]
		public void InMemoryStoreTest()
		{
			using (var store = new InMemoryStore())
			{
				store.Register<Author>();
				IUnitOfWork uow = store as IUnitOfWork;
				const int authorCount = 100;
				Parallel.For(0, authorCount, i =>
				{
					uow.Add(new Author(i, $"Author {i}"));
				});
				ILinqProvider linq = store as ILinqProvider;

				Assert.AreEqual(authorCount, linq.Query<Author>().Count());
				int id = 3;
				Assert.AreEqual(1, linq.Query<Author>().Where(a => a.Id == id).Count());
				Assert.AreEqual(id, linq.Query<Author>().Where(a => a.Id == id).FirstOrDefault().Id);
				Assert.AreEqual($"Author {id}", linq.Query<Author>().Where(a => a.Id == id).FirstOrDefault().Name);

				var author = new Author(id, $"Author#{id}");
				uow.Update(author);
				uow.Commit();

				Assert.AreEqual(1, linq.Query<Author>().Where(a => a.Id == id).Count());
				Assert.AreEqual(author.Id, linq.Query<Author>().Where(a => a.Id == id).FirstOrDefault().Id);
				Assert.AreEqual(author.Name, linq.Query<Author>().Where(a => a.Id == id).FirstOrDefault().Name);

				Parallel.For(0, 100, i =>
				{
					var updAuthor = new Author(id, $"Author#{i}");
					uow.Update(updAuthor);
				});
			}


		}

		[TestMethod]
		public void FantasticSpec_Returns_Fantastic()
		{
			//Arrange
			var qrit = new FantasticQrit { Popular = true };
			var spec = new FantasticSpec();

			//Act
			//System.Diagnostics.Debug.WriteLine($"FantasticSpec:{spec.Expression(qrit)}");
			var actual = books.AsQueryable().Where( spec.Expression(qrit) );

			//Assert
			Assert.IsTrue(actual.Count() > 0);
			Assert.IsTrue(actual.All(b => 
				b.BookGenres.Any(g=>Genre.FantasticGenresList.Any(fg=>fg==g.Genre.Name)))
			);
			Assert.IsTrue(actual.All(b => b.Raiting>5));
		}

		[TestMethod]
		public void TestRulesOnList()
		{

			// Популярные книги
			var popularBooks = books.AsQueryable()
				.Where(Book.Rules.PopularBook && Book.Rules.ElectronicBook);

			Assert.IsTrue(popularBooks.Count() > 0);
			foreach (var book in popularBooks)
			{
				Assert.IsTrue(book.Is(Book.Rules.PopularBook));
				Console.WriteLine(book.ToString());
			}

			var popularAuthors = authors.AsQueryable()
				.Where(a => a.Books.Any(b => b.Raiting > 5));

			Assert.IsTrue(popularAuthors.Count() > 0);
			foreach (var author in popularAuthors)
				Console.WriteLine(author.ToString());

			var popularAuthorsByRule = authors.AsQueryable()
				.Where(a => a.Books.AsQueryable().Any(Book.Rules.PopularBook));

			Assert.IsTrue(popularAuthors.Count() > 0);
			foreach (var author in popularAuthorsByRule)
				Console.WriteLine(author.ToString());

			// Динамическое построение 
			MethodInfo method = typeof(Enumerable).GetMethods()
				.Where(m => m.Name == "Any" && m.GetParameters().Length == 2)
				.Single()
				.MakeGenericMethod(typeof(Book));

			ParameterExpression authorParameter = Expression.Parameter(typeof(Author), "a");

			var expr2 = Expression.Lambda<Func<Author, bool>>(
				Expression.Call(
					method,
					Expression.Property(authorParameter, typeof(Author).GetProperty("Books")),
					Book.Rules.PopularBook)
				, authorParameter);

			System.Diagnostics.Debug.WriteLine(expr2);
			Console.WriteLine("Expr Query Result");

			var exprQuery = authors.AsQueryable().Where(expr2);
			foreach (var author in exprQuery)
				Console.WriteLine(author.ToString());
			//Book book = new Book(1, "War", 1, new Author(1));

			//Expression<Func<Book, Object>> expr = (b=>b.Name);
			//Expression<Func<Book, Object>> exprNull = (b => null);
			//var exprDef = Expression.Default(typeof(Object));
			//var exprPar = Expression.Parameter(typeof(Book), "b");
			//var exprLambda =  Expression.Lambda<Func<Book, Object>>(exprDef, exprPar);
			//var res = exprLambda.Compile().Invoke(book);

			//if (expr.Body.NodeType == ExpressionType.Constant || expr.Body.NodeType == ExpressionType.Convert)
			//	Console.WriteLine(exprNull.Body.NodeType.ToString());

			//var ddd = expr.Compile().Invoke(book);
			//Assert.AreEqual("War", (String)ddd);
		}

		[TestMethod]
		public void TestLazy()
		{
			Expression<Func<String, bool>> IsEven = (x) => ( Int32.Parse(x) %2 )==0;

			var b = IsEven.AsFunc();
			Console.WriteLine(b.GetType());
			Console.WriteLine( b.ToString() );
			Assert.IsTrue(b.Invoke("2"));

			var c = IsEven.AsFunc();
			Assert.IsFalse(c.Invoke("3"));

			Expression<Func<String, bool>> IsEven1 = (x) => (Int32.Parse(x) % 2) == 0;

			var d = IsEven1.AsFunc();

			Assert.IsFalse(d.Invoke("5"));
			Assert.IsTrue(d.Invoke("4"));

			var e = IsEven.AsFunc();

			Book b1 = new Book(1, "1") { Raiting=6};
			b1.AddAuthor(new Author(1, "author1"));
			Book.Rules.PopularBook.IsSatisfiedBy(b1);
			Book.Rules.PopularBook.IsSatisfiedBy(b1);
			Book.Rules.PopularBook.IsSatisfiedBy(b1);
			Book.Rules.PopularBook.IsSatisfiedBy(b1);
		}

		readonly ConcurrentDictionary<String, Lazy<String>> _cache = new ConcurrentDictionary<String, Lazy<String>>();

		private static String RunLongRunningOperation(string operationId)
		{
			// Running real long-running operation
			// ...
			Thread.Sleep(10);
			return operationId;
		}
		public String RunOperationOrGetFromCache(string operationId)
		{
			return _cache.GetOrAdd(
				operationId,
				id => new Lazy<String>(() => RunLongRunningOperation(id)) ).Value;
		}

	}


}
