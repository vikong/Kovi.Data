using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Kovi.Data.Cqrs;
using Kovi.Data.Cqrs.Linq;
using Kovi.LinqExtensions;
using Kovi.LinqExtensions.Expressions;
using Kovi.LinqExtensions.Specification;

namespace Data.Cqrs.Test.EF
{
	public class IdQriteria : IdQriteria<Int32>
	{ }

	public class NameQriteria : IQriteria
	{
		public String Name { get; set; }
	}

	/// <summary>
	/// Книги по автору или наименованию
	/// </summary>
	public class BookOrAuthorQriteria : NameQriteria
	{}

	public class BookOrAuthorFilterSpec 
		: IFilterSpec<BookOrAuthorQriteria, Book>
	{
		public Expression<Func<Book, Boolean>> Expression(BookOrAuthorQriteria qrit)
			=> (b) => b.Name.Contains(qrit.Name) || b.BookAuthors.Any(a => a.Author.Name.Contains(qrit.Name));
	}

	public class BookOrAuthorQuery : SpecLinqQuery<BookOrAuthorQriteria, Book, IEnumerable<BookDto>>
	{
		public BookOrAuthorQuery(ILinqRequestHandler handler, IEnumLinqConvertor<BookDto> convertor)
			: base(new BookOrAuthorFilterSpec(), handler, convertor)
		{ }
	}


	/// <summary>
	/// Книги с жанром фантастики и популярные.
	/// </summary>
	public class FantasticQrit : IQriteria
	{
		public Boolean Popular { get; set; }
		public Boolean Electronic { get; set; }
	}

	/// <summary>
	/// Запрос, демонстрирующий использование композицию бизнес-правил в функциональном стиле.
	/// </summary>
	public class FantasticSpec : IFilterSpec<FantasticQrit, Book>
	{
		public Expression<Func<Book, Boolean>> Expression(FantasticQrit qrit)
			=> Book.Rules.FantasticBook
				.AndIf(qrit.Popular, Book.Rules.PopularBook)
				.AndIf(qrit.Electronic, Book.Rules.ElectronicBook);
	}

	public class FantasticFuncQuery : SpecLinqQuery<FantasticQrit, Book, IEnumerable<BookDto>>
		,IQuery<FantasticQrit, IEnumerable<BookDto>>
	{
		public FantasticFuncQuery(ILinqRequestHandler handler, IEnumLinqConvertor<BookDto> convertor) 
			: base(new FantasticSpec(), handler, convertor)
		{}
	}

	public class FilteredAuthorLinquery : SpecLinqQuery<EmptyQriteria, Author, IEnumerable<Author>>
	{
		public FilteredAuthorLinquery(ILinqRequestHandler handler, IEnumLinqConvertor<Author> convertor)
			: base(LinqSpec<Author>.Filter<EmptyQriteria>((a) => a.Id > 1), handler, convertor)
		{ }
	}


	public class AllEntityLinquery<TEntity, TResponse> : SpecLinqQuery<IQriteria, TEntity, IEnumerable<TResponse>>
	where TEntity : class
	{
		public AllEntityLinquery(ILinqRequestHandler handler, IEnumLinqConvertor<TResponse> convertor)
			: base(LinqSpec<TEntity>.All(), handler, convertor)
		{
		}
	}


	/// <summary>
	/// Книги, которые написаны в жанре, начинающимся со строки критерия.
	/// </summary>
	public class BookInGenreQrit : NameQriteria {}


	public class SofisticatedRequest
		: ILinqRequest<BookInGenreQrit, IEnumerable<Object>>
	{
		public IEnumerable<Object> Request(ILinqProvider linqProvider, BookInGenreQrit qrit)
		{
			// некая сложная выборка, для которой недостаточно Expression
			var q1 = from b in linqProvider.Query<Book>()
					 where b.BookGenres.Any(bg => bg.Genre.Name.StartsWith(qrit.Name))
					 select new { b.Id, BookName = b.Name, b.BookGenres };

			var d1 = q1.ToArray();

			var q2 = from b in d1
					 select new
					 {
						 b.BookName,
						 Genre = b.BookGenres.AsQueryable().Aggregate("Genre(s): ", (g, next) => g + ", " + next.Genre.Name)
					 };

			return q2;
		}

		public Task<IEnumerable<Object>> RequestAsync(ILinqProvider linqProvider, BookInGenreQrit qrit)
		{
			throw new NotImplementedException();
		}
	}

	public class SofQuer : BaseRequestQuery<BookInGenreQrit, IEnumerable<Object>>
	{
		public SofQuer(ILinqRequestHandler requestHandler)
			:base(requestHandler)
		{}
		public override ILinqRequest<BookInGenreQrit, IEnumerable<Object>> LinqRequest 
			=> new SofisticatedRequest();
	}

	public class SofisticatedQuery 
		: RequestQuery<BookInGenreQrit, IEnumerable<Object>>
		, IQuery<BookInGenreQrit, IEnumerable<Object>>
	{
		public SofisticatedQuery(ILinqRequestHandler requestHandler) 
			: base(requestHandler)
		{ }

		public override IEnumerable<Object> Request(ILinqProvider linqProvider, BookInGenreQrit qrit)
		{
			// некая сложная выборка, для которой недостаточно Expression
			var q1 = from b in linqProvider.Query<Book>()
					 where b.BookGenres.Any(bg => bg.Genre.Name.StartsWith(qrit.Name))
					 select new { b.Id, BookName = b.Name, b.BookGenres };

			var d1 = q1.ToArray();

			var q2 = from b in d1
					 select new
					 {
						 b.BookName,
						 Genre = b.BookGenres.AsQueryable().Aggregate("Genre(s): ", (g, next) => g + ", " + next.Genre.Name)
					 };

			return q2;
		}
	}


	public class AuthorNameQuery : Linquery<NameQriteria, Author, IEnumerable<AuthorDto>>
	{
		public AuthorNameQuery(ILinqRequestHandler handler, IEnumLinqConvertor<AuthorDto> linqConvertor) 
			: base(handler, linqConvertor)
		{}

		public override IQueryable<Author> Query(IQueryable<Author> query, NameQriteria qrit)
			=> query.Where(a => a.Name.Contains(qrit.Name));
	}


	/// <summary>
	/// Пример простой спецификации.
	/// </summary>
	public class AuthorNameFilterSpec : IFilterSpec<NameQriteria, Author>
	{
		public Expression<Func<Author, Boolean>> Expression(NameQriteria qrit)
			=> (a) => a.Name.StartsWith(qrit.Name);
	}


	public class AuthorNameLinqSpec : ILinqSpec<NameQriteria>
	{
		public IQueryable Query(ILinqProvider linqProvider, NameQriteria qrit)
			=> linqProvider.Query<Author>()
			.Where(a=>a.Name.StartsWith(qrit.Name));
	}

	public class AuthorNameFuncSpec : FuncLinqSpec<Author, NameQriteria>
	{
		public AuthorNameFuncSpec() 
			: base((qrit) => (author) => author.Name.StartsWith(qrit.Name))
		{}
	}



	public class PopularAuthorQriteria : IQriteria
	{
		/// <summary>
		/// Имя автора
		/// </summary>
		public String Name { get; set; }
	}

	/// <summary>
	/// Спецификация "популярные авторы" (у которых все книги популярные)
	/// Демонстрирует использование правила "популярная книга".
	/// </summary>
	public class PopularAuthorSpec : IFilterSpec<PopularAuthorQriteria, Author> 
	{
		public Expression<Func<Author, Boolean>> Expression(PopularAuthorQriteria qrit)
		{
			Expression<Func<Author, Boolean>> expr =
				(a) => a.Books.AsQueryable().Any(Book.Rules.PopularBook);

			return expr.AndIf(qrit.Name.Length > 0, (a) => a.Name.StartsWith(qrit.Name));
		}
	}

	public class PopularAuthorQuery : SpecLinqQuery<PopularAuthorQriteria, Author, IEnumerable<Author>>
		, IQuery<PopularAuthorQriteria, IEnumerable<Author>>
	{
		public PopularAuthorQuery(ILinqRequestHandler requestHandler, IEnumLinqConvertor<Author> convertor) 
			: base(new PopularAuthorSpec(), requestHandler, convertor)
		{
		}
	}


}
