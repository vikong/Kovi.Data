using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Data.Cqrs.Test.EF;

using Kovi.Data.Cqrs;
using Kovi.Data.Cqrs.Linq;
using Kovi.LinqExtensions;
using Kovi.LinqExtensions.Expressions;
using Kovi.LinqExtensions.Specification;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using Ninject;

namespace Data.Cqrs.Test
{

    [TestClass]
	public class QueryTest
	{
		private readonly IServiceLocator serviceLocator;
		private readonly ICqService handlerLocator;

		public QueryTest()
		{
			IKernel ninject = new StandardKernel(new FactoriesModule(), new QueriesModule(), new AutoMapperModule());
			serviceLocator = new NinjectServiceLocator(ninject);
			//handlerLocator = new CqServiceLocator(serviceLocator);
			//IUnitOfWorkFactory uowFactory = ninject.Get<IUnitOfWorkFactory>();

		}

		[TestMethod]
		public void ServiceLocatorTest()
		{
			var q = serviceLocator.GetInstance<CreateBookCheckQuery>();
			Assert.IsNotNull(q);
			//ILinqProvider linq = serviceLocator.GetInstance<ILinqProvider>();
			//Assert.IsNotNull(linq);

			//var query =serviceLocator.GetInstance<IQuery<BaseIdQriteria, AuthorDto>>();
			//var actual = serviceLocator.GetInstance<IQuery<IHasId, AuthorDto>>();
			//var actual = serviceLocator.GetInstance<IQuery<BaseIdQriteria, AuthorDto>>();
			//var actual = serviceLocator.GetInstance< IQuery<EmptyQriteria, IEnumerable<AuthorDto>> >();
			//var queryLocator = handlerLocator
			//	.For<IEnumerable<Book>>();
			//Assert.IsNotNull(queryLocator);

		}

		[TestMethod]
		public void ServiceLocator_ForIdQriteria_ReturnsIdQuery()
		{
			var actual = serviceLocator.GetInstance<IByIdQuery<AuthorDto>>();

			Assert.IsInstanceOfType(actual, typeof(ConventionIdQuery<AuthorDto>));

		}

		[TestMethod]
		public void ServiceLocator_ForPageQriteria_ReturnsConventionPagedQuery()
		{


			var actual = serviceLocator.GetInstance<IQuery<IPageQriteria<AuthorDto>, IPage<AuthorDto>>>();

			Assert.IsInstanceOfType(actual, typeof(ConventionPagedQuery<AuthorDto, AuthorDto>));

		}


		[TestMethod]
		public void ServiceLocator_ForTypedQriteria_ReturnsQuery()
		{
			var actual = serviceLocator.GetInstance<IQuery<FantasticQrit, IEnumerable<BookDto>>>();

			Assert.IsInstanceOfType(actual, typeof(FantasticFuncQuery));

		}

		[TestMethod]
		public void ServiceLocator_ForBookOrAuthorQriteria_ReturnsQuery()
		{
			var actual = serviceLocator.GetInstance<IQuery<BookOrAuthorQriteria, IEnumerable<BookDto>>>();

			Assert.IsInstanceOfType(actual, typeof(BookOrAuthorQuery));

		}

		[TestMethod]
		public void Query_ById_Returns_EntityWithId()
		{
			//Arrange
			var qrit = new IdQriteria { Id = 2 };

			//Act
			var actual = handlerLocator
				.For<AuthorDto>()
				.Find(qrit);

			//Assert
			Assert.AreEqual(qrit.Id, actual.Id);
		}

		[TestMethod]
		public void Query_ByStringId_Returns_EntityWithId()
		{
			//Arrange
			var qrit = new IdQriteria<String> { Id = "PITER" };

			//Act
			var actual = handlerLocator
				.For<PublisherDto>()
				.Find(qrit);

			//Assert
			Assert.AreEqual(qrit.Id, actual.Id);
		}

		[TestMethod]
		public void QueryForPageQriteria_Returns_Page()
		{
			//Arrange
			const int authorCount = 25;

			using (var UoWStub = Stub.CreateMemoryUoW(typeof(Author))
				.Add(
					authorCount,
					(i) => new Author(i, "A" + (i % 2 == 0 ? "_" : "-") + Convert.ToString(i).PadLeft(2, '0'))
				))
			{

				var linqProviderFactoryStub = UoWStub.LinqProviderFactory();

				var handler = new LinqRequestHandler(linqProviderFactoryStub);

				var convertor = serviceLocator.GetInstance<IPageConvertor<AuthorDto>>();

				var projector = serviceLocator.GetInstance<IProjector>();

				var qrit = new PageQriteria<AuthorDto>
				{
					PageNo = 3,
					PageSize = 5,
					OrderBy = "Name",
					Subject = new AuthorDto { Name = "A" }
				};


				var query = new ConventionPagedQuery<AuthorDto, AuthorDto>(handler, convertor);

				var expected = linqProviderFactoryStub.Create()
					.Query<Author>()
					.Where(a => a.Name.StartsWith(qrit.Subject.Name))
					.OrderBy(a => a.Name)
					.Skip((qrit.PageNo - 1) * qrit.PageSize)
					.Take(qrit.PageSize)
					.Project<AuthorDto>(projector)
					.ToArray();

				//Act
				var actual = query.Ask(qrit);

				//Assert
				CollectionAssert.AreEqual(expected, actual.Members.ToArray(), new AuthorDtoComparer());

			}
		}


		[TestMethod]
		public void Sofisticated_Test()
		{
			var qrit = new BookInGenreQrit { Name = "ф" };
			var actual = handlerLocator
				.For<IEnumerable<Object>>()
				.Ask(qrit);

			Assert.IsNotNull(actual);
			foreach (var e in actual)
				Console.WriteLine(e.ToString());
		}

		[TestMethod]
		public async Task EFQueryAsyncTest()
		{
			int cntBook = 0;
			using (ILinqProvider linq = serviceLocator.GetInstance<ILinqProviderFactory>().Create())
			{
				var books = linq.Query<Book>().ToList();
				Assert.IsNotNull(books);
				cntBook = books.Count();
				Assert.IsTrue(cntBook > 0);
			}

			var allBooks = await handlerLocator
				.For<IEnumerable<BookDto>>()
				.AskAsync(new BookOrAuthorQriteria());

			var idQrit = new IdQriteria { Id = 2 };
			var book = await handlerLocator
				.For<BookDto>()
				.AskAsync(idQrit);

			Assert.IsNotNull(allBooks);
			Assert.IsInstanceOfType(allBooks, typeof(IEnumerable<BookDto>));
			Assert.IsTrue(allBooks.Count() > 0);
			Assert.IsTrue(allBooks.Count() == cntBook);

			Assert.IsNotNull(book);
			Assert.IsInstanceOfType(book, typeof(BookDto));
			Assert.AreEqual(idQrit.Id, book.Id);
		}

		[TestMethod]
		public void DtoQueryTest()
		{
			var result = handlerLocator
				.For<IEnumerable<BookDto>>()
				.Ask(new BookOrAuthorQriteria { Name = "Тол" });

			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(IEnumerable<BookDto>));
			Assert.IsTrue(result.Count() > 0);

			StringBuilder sb = new StringBuilder();
			foreach (var b in result)
			{
				sb.Append($"{b}");
				//sb.Append("Автор(ы): ");
				//foreach (var a in b.Authors)
				//	sb.Append(a.Name);
				sb.AppendLine(";");
			}
			//System.Diagnostics.Debug.WriteLine(sb.ToString());
			Console.Write(sb.ToString());

			var result1 = handlerLocator
				.For<IEnumerable<Book>>()
				.Ask(new EF.BookOrAuthorQriteria { Name = "Тол" });

			Assert.IsNotNull(result1);
			Assert.IsInstanceOfType(result1, typeof(IEnumerable<Book>));
			Assert.IsTrue(result1.Count() > 0);
		}


		[TestMethod]
		public void Query_ForFantastic_Returns_OnlyFantastic()
		{
			//Arrange
			var qrit = new FantasticQrit { Popular = true };

			//Act
			var actual = handlerLocator
				.For<IEnumerable<BookDto>>()
				.Ask(qrit);

			//Assert
			Assert.IsTrue(actual.Count()>0);
			Assert.IsTrue(actual.All(b => b.Raiting>5));
			Assert.IsTrue(actual.All(b =>
				b.BookGenres.Any(g => Genre.FantasticGenresList.Any(fg => fg == g.Genre.Name))));

		}

		[TestMethod]
		public void Query_ForPopularAuthor_Returns_Popular()
		{
			var qrit = new EF.PopularAuthorQriteria { Name = "Че" };

			var popularAuthors = handlerLocator
				.For<IEnumerable<Author>>()
				.Ask(qrit);

			Assert.IsTrue(popularAuthors.All(a=>a.Name.StartsWith(qrit.Name) && a.Books.All(b=>b.Is(Book.Rules.PopularBook))));
		}

		[TestMethod]
		public void SpecLinquery_Default_ReturnsAll()
		{
			//Arrange
			const int numOfAuthors = 20;
			using (var uow = Stub.CreateMemoryUoW(typeof(Author)))
			{
				uow.AddAuthors(numOfAuthors);

				var linqProviderFactoryStub = new Mock<ILinqProviderFactory>();
				linqProviderFactoryStub.Setup(a => a.Create(It.IsAny<String>(), It.IsAny<String>())).Returns(uow.Linq);

				var handler = new LinqRequestHandler(linqProviderFactoryStub.Object);

				IQuery<IQriteria, IEnumerable<AuthorDto>> query = new SpecLinqQuery<IQriteria, Author, IEnumerable<AuthorDto>>(
					LinqSpec<Author>.All(),
					handler,
					serviceLocator.GetInstance<IEnumLinqConvertor<AuthorDto>>()
				);

				//Act
				var actual = query.Ask(new NameQriteria());

				//Assert
				Assert.AreEqual(numOfAuthors, actual.Count());
			}
		}


		//[TestMethod]
		//public void Linquery_WithSpec()
		//{
		//	//Arrange
		//	var uow = CreateMemoryUoW(typeof(Author));

		//	const int numOfAuthorsA = 10, numOfAuthorsB = 12;
		//	var authorAQrit = new NameQriteria { Name = "A." };
		//	var authorBQrit = new NameQriteria { Name = "B." };
		//	AddAuthors(uow, numOfAuthorsA, authorAQrit.Name);
		//	AddAuthors(uow, numOfAuthorsB, authorBQrit.Name);

		//	var linqProviderFactoryStub = new Mock<ILinqProviderFactory>();
		//	linqProviderFactoryStub.Setup(a => a.Create(It.IsAny<String>(), It.IsAny<String>())).Returns(uow.Linq);
		//	var handler = new LinqSpecHandler(linqProviderFactoryStub.Object);
		//	var convertor = serviceLocator.GetInstance<INativeEnumLinqConvertor<Author>>();
		//	var q = serviceLocator.GetInstance<IQuery<NameQriteria, IEnumerable<Author>>>();
		//	Assert.IsInstanceOfType(q, typeof(Linquery2<NameQriteria, IEnumerable<Author>>));

		//	var query = new Linquery2<NameQriteria, IEnumerable<Author>>(new AuthorNameSpec(), handler, convertor);
		//	//Act
		//	var actual = query.Ask(authorAQrit);
		//	//var actual = handlerLocator
		//	//	.For<IEnumerable<Author>>()
		//	//	.Ask(authorAQrit);


		//	//Assert
		//	Assert.AreEqual(numOfAuthorsA, actual.Count());

		//}

		[TestMethod]
		public void DapperQuery_ForAll_ReturnsAll()
		{
			var qrit = new Dapper.DapperAllBookQriteria();

			var result = handlerLocator
				.For<IEnumerable<BookDto>>()
				.Ask(qrit);

			Assert.IsTrue(result.Count() > 0);
		}

		[TestMethod]
		public async Task DapperAsyncQuery_ForAll_ReturnsAll()
		{
			var qrit = new Dapper.DapperAllBookQriteria();

			var result = await handlerLocator
				.For<IEnumerable<BookDto>>()
				.AskAsync(qrit);

			Assert.IsTrue(result.Count() > 0);

		}

		[TestMethod]
		public void DapperQuery_ForId_ReturnsBookWithId()
		{
			var qrit = new Dapper.DapperBookByIdQriteria { Id = 2 };

			var result = handlerLocator
				.For<BookDto>()
				.Ask(qrit);

			Assert.AreEqual(2, result.Id);

		}

	}
}
