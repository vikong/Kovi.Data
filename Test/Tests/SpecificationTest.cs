using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using Kovi.Data.Cqrs;
using EF = Data.Cqrs.Test.EF;
using Dapper = Data.Cqrs.Test.Dapper;

using Ninject;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Kovi.LinqExtensions;
using Kovi.LinqExtensions.Specification;
using Kovi.LinqExtensions.Expressions;
using Kovi.Data.Cqrs.Linq;
using System.Linq.Expressions;
using System.Diagnostics;
using Data.Cqrs.Test.EF;

namespace Data.Cqrs.Test
{
	[TestClass]
	public class SpecificationTest
	{
		private IUnitOfWork CreateMemoryUoW(params Type[] types)
		{
			var uow = new InMemoryStore();
			foreach (var t in types)
				uow.Register(t);
			return uow;
		}

		private void AddAuthors(IUnitOfWork uow, int authorCount, string authorPred = "Author #")
		{
			var query = uow.Linq.Query<Author>();
			int idx = query.Any() ? query.Max((a) => a.Id) + 1 : 1;
			Parallel.For(idx, idx + authorCount, i =>
			{
				uow.Add(new Author(i, authorPred + Convert.ToString(i).PadLeft(3, '0')));
			});

		}

		[TestMethod]
		public void ExpressionSpec_Default_ReturnsAll()
		{
			//Arrange
			const int numOfAuthors = 5;
			using (var uow = CreateMemoryUoW(typeof(Author)))
			{
				AddAuthors(uow, numOfAuthors);

				ILinqSpec<NameQriteria, Author> spec = new ExpressionLinqSpec<Author, NameQriteria>();

				//Act
				var actual = spec.Query(uow.Linq.Query<Author>(), null);

				//Assert
				Assert.AreEqual(numOfAuthors, actual.Count());
			}
		}

		[TestMethod]
		public void FantasticBookSpec_ReturnsFantastic()
		{
			//Arrange
			//const int numOfBooks = 5;
			using (var uow = CreateMemoryUoW(new Type[] { typeof(Book), typeof(Author), typeof(Genre) } ))
			{
				Genre genre = new Genre("жанр"); 
				Genre genreFant = new Genre("фантастика"); 

				Book book;
				book = new Book(1,"Book1");
				book.Raiting = 1;
				book.AddGenre(genre);
				uow.Add(book);

				book = new Book(2, "Book2");
				book.Raiting = 9;
				book.AddGenre(genreFant);
				uow.Add(book);

				book = new Book(3, "Book3");
				book.Raiting = 1;
				book.AddGenre(genre);
				book.AddGenre(genreFant);
				uow.Add(book);

				book = new Book(4, "Book4");
				book.Raiting = 6;
				book.AddGenre(genre);
				book.AddGenre(genreFant);
				uow.Add(book);

				book = new Book(5, "Book5");
				book.Raiting = 1;
				book.AddGenre(genre);
				uow.Add(book);

				var qrit = new FantasticQrit { Popular = true };
				var spec = new FantasticSpec();
				ILinqSpec<FantasticQrit, Book> linqSpec = new ExpressionLinqSpec<Book, FantasticQrit>(spec);
				//IFilterSpec<FantasticQrit, Book> spec = new FantasticSpec();

				//Act
				var actual = uow.Linq.Query<Book>().Apply(linqSpec,qrit);

				//Assert
				Assert.IsTrue(actual.Count()>0);
				Assert.IsTrue(actual.All(b => Book.Rules.PopularBook.IsSatisfiedBy(b)));
			}
		}


		[TestMethod]
		public void EmptyLinqSpec_ReturnsAll()
		{
			//Arrange
			const int numOfAuthors = 5;
			using (var uow = CreateMemoryUoW(typeof(Author)))
			{
				AddAuthors(uow, numOfAuthors);
				var spec = LinqSpec<Author>.All();

				//Act
				var actual = spec.Query(uow.Linq.Query<Author>(), null);

				//Assert
				Assert.AreEqual(numOfAuthors, actual.Count());
			}
		}

		[TestMethod]
		public void ExpressionSpec_WithExpression()
		{
			//Arrange
			const int numOfAuthors = 20, id = 10;
			using (var uow = CreateMemoryUoW(typeof(Author)))
			{
				AddAuthors(uow, numOfAuthors);

				//var linqProviderFactoryStub = new Mock<ILinqProviderFactory>();
				//linqProviderFactoryStub.Setup(a => a.Create(It.IsAny<String>(), It.IsAny<String>())).Returns(uow.Linq);
				//ILinqQueryHandler handler = new LinqQueryHandler(linqProviderFactoryStub.Object);

				var spec = LinqSpec<Author>.Filter<IQriteria>((a) => a.Id > id);

				//Act
				var actual = spec.Query(uow.Linq.Query<Author>(), new NameQriteria());

				//Assert
				Assert.IsFalse(actual.Any(a => a.Id <= id));
			}
		}

		[TestMethod]
		public void FuncSpec_WithCustomFunc()
		{
			using (var uow = CreateMemoryUoW(typeof(Author)))
			{
				const int numOfAuthorsA = 10, numOfAuthorsB = 12;
				var authorAQrit = new EF.NameQriteria { Name = "A." };
				var authorBQrit = new EF.NameQriteria { Name = "B." };
				AddAuthors(uow, numOfAuthorsA, authorAQrit.Name);
				AddAuthors(uow, numOfAuthorsB, authorBQrit.Name);

				//Arrange
				var spec = LinqSpec<Author>.FilterFunc<EF.NameQriteria>(
					(qrit) => author => author.Name.StartsWith(qrit.Name)
				);

				//Act
				var actual = spec.Query(uow.Linq.Query<Author>(), authorBQrit);

				//Assert
				Assert.AreEqual(numOfAuthorsB, actual.Count());
				Assert.IsTrue(actual.Any(a => a.Name.StartsWith(authorBQrit.Name)));
			}
		}

		[TestMethod]
		public void ConventionIdLinqSpec_Returns_EntityWithId()
		{
			//Arrange
			const int numOfAuthors = 10;
			using (var uow = CreateMemoryUoW(typeof(Author)))
			{
				AddAuthors(uow, numOfAuthors);

				var spec = new ConventionIdLinqSpec<Author>();

				var qrit = new IdQriteria { Id = 2 };

				//Act
				var actual = spec.Query(uow.Linq, qrit);

				//Assert
				Assert.AreEqual(qrit.Id, actual.Cast<Author>().Single().Id);
			}
		}

		class AuthorQrit : IQriteria
		{
			public String Name { get; set; }
		};

		[TestMethod]
		public void ConventionSpec_Returns_Filtered()
		{
			//Arrange
			using (var uow = CreateMemoryUoW(typeof(Author)))
			{
				uow.Add(new Author(1, "A.A.Author"));
				uow.Add(new Author(2, "A.B.Author"));
				uow.Add(new Author(3, "C.D.Author"));
				uow.Add(new Author(4, "C.D.Author"));
				uow.Add(new Author(5, "E.E.Author"));

				var qrit = new AuthorQrit { Name = "C" };
				var spec = LinqSpec<Author>.ByConvention(qrit);

				//Act
				var actual = spec.Query(uow.Linq.Query<Author>(), qrit);

				//Assert
				Assert.IsTrue(actual.All(a => a.Name.StartsWith(qrit.Name)));
			}
		}

		[TestMethod]
		public void ConventionPageSpec_Returns_Filtered()
		{
			//Arrange
			using (var uow = CreateMemoryUoW(typeof(Author)))
			{
				uow.Add(new Author(1, "A.A.Author"));
				uow.Add(new Author(2, "A.B.Author"));
				uow.Add(new Author(3, "C.D.Author"));
				uow.Add(new Author(4, "C.D.Author"));
				uow.Add(new Author(5, "E.E.Author"));

				var qrit = new PageQriteria<AuthorDto>
				{
					Subject = new AuthorDto { Name = "C" },
					OrderBy = "Name",
					PageNo = 2,
					PageSize = 2
				};
				var spec = new ConventionPagedSpec<Author, AuthorDto>(); //LinqSpec<Author>.ByConvention(qrit);

				//Act
				var actual = spec.Query(uow.Linq, qrit)
					.Cast<Author>();

				//Assert
				Assert.IsTrue(actual.All(a => a.Name.StartsWith(qrit.Subject.Name)));
			}
		}

		[TestMethod]
		public void ConventionPageSpec_Returns_Ordered()
		{
			//Arrange
			using (var uow = CreateMemoryUoW(typeof(Author)))
			{
				uow.Add(new Author(1, "A.D.Author"));
				uow.Add(new Author(2, "A.B.Author"));
				uow.Add(new Author(3, "B.A.Author"));
				uow.Add(new Author(4, "A.C.Author"));
				uow.Add(new Author(5, "B.E.Author"));

				var qrit = new PageQriteria<AuthorDto>
				{
					Subject = new AuthorDto { Name = "A" },
					OrderBy = "Name",
					PageNo = 2,
					PageSize = 2
				};
				var spec = new ConventionPagedSpec<Author, AuthorDto>();

				//Act
				var actual = spec.Query(uow.Linq, qrit)
					.Cast<Author>();

				//Assert
				Assert.IsTrue(actual.All(a => a.Name.StartsWith(qrit.Subject.Name)));
				CollectionAssert.AreEqual(actual.ToArray(),
					uow.Linq.Query<Author>().Where(a => a.Name.StartsWith(qrit.Subject.Name)).OrderBy(a => a.Name).ToArray());
			}
		}

	}
}
