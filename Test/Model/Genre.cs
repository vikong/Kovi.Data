using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Kovi.LinqExtensions.Expressions;
using Kovi.LinqExtensions.Specification;

namespace Data.Cqrs.Test
{
	/// <summary>
	/// Жанр
	/// </summary>
	public class Genre : BaseEntity
	{
		public readonly static List<String> FantasticGenresList = new List<String> { "фантастика", "фэнтэзи" };

		public String Name { get; set; }

		//public virtual ICollection<Book> Books { get; set; }
		public virtual ICollection<BookGenre> BookGenresCollection { get; set; }

		[Obsolete("Only for model binders and EF, don't use it in your code", true)]
		public Genre()
		{
			BookGenresCollection = new HashSet<BookGenre>();
		}

		public Genre(String genreName):base(0)
		{
			Name=genreName.Trim().ToLower();
		}

		public override string ToString() => Name;

		internal static class Rules
		{
			/// <summary>
			/// Правило, определяющее фантастический жанр
			/// </summary>
			public static readonly Rule<Genre> FantasticGenre =
				Rule<Genre>.InList( FantasticGenresList, (g) => g.Name );
		}
	}

	public class BookGenre
	{
		public Int32 BookId { get; protected set; }
		public virtual Book Book { get; protected set; }

		public Int32 GenreId { get; protected set; }
		public virtual Genre Genre { get; protected set; }

		[Obsolete("Only for model binders and EF, don't use it in your code", true)]
		public BookGenre()
		{}

		public BookGenre(Book book, Genre genre)
		{
			Book = book;
			BookId = book.Id;
			Genre = genre;
			GenreId = genre.Id;
		}

		public BookGenre(Book book, Int32 genreId)
		{
			Book = book;
			BookId = book.Id;
			GenreId = genreId;
		}

		internal static class Rules
		{
			/// <summary>
			/// Правило, относящее жанр книги к жанру фантастики.
			/// </summary>
			public static readonly Rule<BookGenre> FantasticBookGenre
				=Rule<BookGenre>.Create(Genre.Rules.FantasticGenre, (g) => g.Genre);
				//= Genre.Rules.FantasticGenreRule.For<BookGenre>(g => g.Genre);

		}
	}

}