using System;
using System.Collections.Generic;
using System.Linq;
using Kovi.Data.Cqrs;
using Kovi.Data.Dapper;

namespace Data.Cqrs.Test.Dapper
{
	/// <summary>
	/// Все книги "плоским" списком
	/// </summary>
	public class DapperAllFlatBookQriteria : IQriteria
	{ }

	/// <summary>
	/// Книги в виде "плоского" списка
	/// </summary>
	public sealed class DapperAllBooksQuery
		: DapperQueryBase<DapperAllFlatBookQriteria, BookDto, IEnumerable<BookDto>>
	{
		public DapperAllBooksQuery(IDapperQueryHandler<DapperAllFlatBookQriteria, IEnumerable<BookDto>> handler)
			:base(handler)
		{ }

		public override String Sql => 
			@"SELECT * FROM [Book]";

		public override IEnumerable<BookDto> Convert(IEnumerable<BookDto> buffer)
			=> buffer;
	}

	/// <summary>
	/// Все книги как список объектов
	/// </summary>
	public class DapperAllBookQriteria : IQriteria
	{ }

	/// <summary>
	/// Список книг
	/// </summary>
	public sealed class BooksDapperMapQuery
		: DapperQueryBase<DapperAllBookQriteria, BookDto, IEnumerable<BookDto>>
		, IDapperMapQuery<DapperAllBookQriteria, BookDto, IEnumerable<BookDto>>
	{
		public BooksDapperMapQuery(IDapperQueryHandler<DapperAllBookQriteria, IEnumerable<BookDto>> handler)
			:base(handler)
		{ }

		public override String Sql => 
@"select *
from [Book] b
left join [BookAuthor] ba on b.Id=ba.BookId
left join [Author] a on ba.AuthorId=a.Id";

		public override IEnumerable<BookDto> Handle(DapperAllBookQriteria qrit)
		{
			books = new Dictionary<int, BookDto>();
			Param = qrit;
			
			return QueryHandler.Handle(this);
		}
		public Type[] SplitTypes => new Type[] { typeof(Book), typeof(Author) };

		public string SplitOn => "AuthorId";
		private Dictionary<int, BookDto> books;
		public BookDto MapFunc(Object[] par)
		{
			Book b = par[0] as Book;
			Author a = par[1] as Author;
			if (!books.TryGetValue(b.Id, out BookDto bookEntry))
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
		}

		public override IEnumerable<BookDto> Convert(IEnumerable<BookDto> buffer)
			=> buffer.Distinct().ToList();

	}

}