using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kovi.Data.Cqrs;
using Kovi.Data.Cqrs.Infrastructure;
using Kovi.Data.Cqrs.Linq;

namespace Data.Cqrs.Test
{
	public class BaseDto : HasIdBase<Int32>
	{ }

	/// <summary>
	/// Книга
	/// </summary>
	[EntityMap(typeof(Book), MapOptions.DtoToEntity | MapOptions.EntityToDto)]
	public class BookDto: BaseDto
	{
		public String Name { get; set; }
		public ICollection<BookAuthorDto> BookAuthors { get; set; }
		public ICollection<BookGenreDto> BookGenres { get; set; }
		public Boolean HasElectronic { get; set; }
		public Int32 Published { get; set; }
		public Int32? Raiting { get; set; }

		public BookDto()
		{
			BookAuthors = new List<BookAuthorDto>();
			BookGenres = new List<BookGenreDto>();
		}

		//public BookDto(string bookName, Genre type, IEnumerable<AuthorDto> authors):base()
		//{
		//	Name=bookName;
		//	Authors=new List<AuthorDto>(authors);
		//}

		//public BookDto(string bookName, Genre type, AuthorDto author)
		//	: this(bookName, type, new AuthorDto[] {author})
		//{}
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder($"ID:{Id}, Name:\"{Name}\", ");
			sb.Append("Author(s):");
			if (BookAuthors!=null && BookAuthors.Count() > 0)
			{
				sb.Append("[");
				foreach (var bookAuthor in BookAuthors)
					sb.AppendFormat("{0},", bookAuthor.Author != null ? bookAuthor.Author.Name : "not-loaded");
				sb.Remove(sb.Length - 1, 1);
				sb.Append("], ");
			}
			else
				sb.Append("unkonwn, ");

			sb.Append("Genre(s):");
			if (BookGenres != null && BookGenres.Count() > 0)
			{
				sb.Append("[");
				foreach (var bookGenre in BookGenres)
					sb.AppendFormat("{0},", bookGenre.Genre != null ? bookGenre.Genre.Name : "not-loaded");
				sb.Remove(sb.Length - 1, 1);
				sb.Append("], ");
			}
			else
				sb.Append("unkonwn, ");

			sb.Append($"Raiting:{Raiting ?? 0}, ");
			//sb.Append($"Genre:{Genre}");
			return sb.ToString();
		}

	}


	/// <summary>
	/// Автор
	/// </summary>
	[EntityMap(typeof(Author), MapOptions.DtoToEntity | MapOptions.EntityToDto, conventionQuery:typeof(ConventionPagedQuery<,>))]
	public class AuthorDto : BaseDto
	{
		public String Name { get; set; }
		public DateTime? Birthday { get; set; }

	}

	[EntityMap(typeof(BookAuthor))]
	public class BookAuthorDto
	{
		public BookDto Book { get; set; }

		public AuthorDto Author { get; set; }

	}

	/// <summary>
	/// Жанр
	/// </summary>
	[EntityMap(typeof(Genre))]
	public class GenreDto: BaseDto
	{
		public String Name { get; set; }
	}


	[EntityMap(typeof(BookGenre))]
	public class BookGenreDto
	{
		public virtual Book Book { get; set; }

		public virtual Genre Genre { get; set; }

	}

	[EntityMap(typeof(Publisher))]
	public class PublisherDto : HasIdBase<String>
	{
		public String Name { get; set; }
	}

}