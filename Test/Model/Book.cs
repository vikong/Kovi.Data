using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Kovi.LinqExtensions.Specification;

namespace Data.Cqrs.Test
{
	/// <summary>
	/// Книга
	/// </summary>
	public class Book : BaseEntity
	{
		private String _name;
		public String Name
		{
			get { return _name; }
			set { if (String.IsNullOrWhiteSpace(value)) throw new ArgumentNullException("Book Name"); _name = value; }
		}

		/// <summary>
		/// Авторы книги. 
		/// </summary>
		public virtual ICollection<BookAuthor> BookAuthors { get; protected set; }

		public virtual ICollection<BookGenre> BookGenres { get; protected set; }
		
		public String PublisherId { get; set; }
		public virtual Publisher Publisher { get; protected set; }

		public Boolean HasElectronic { get; set; }

		public Int32 Published { get; set; }

		public Int32? Raiting { get; set; }


		#region .ctor
		private void Initialize(IEnumerable<BookAuthor> bookAuthors=null, IEnumerable<BookGenre> bookGenres=null)
		{
			if (bookAuthors != null)
				BookAuthors = new List<BookAuthor>(bookAuthors);
			else
				BookAuthors = new List<BookAuthor>();

			if (bookGenres != null)
				BookGenres = new List<BookGenre>(bookGenres);
			else
				BookGenres = new List<BookGenre>();
		}

		[Obsolete("Only for model binders and EF, don't use it in your code", true)]
		public Book()
		{
			Initialize();
		}

		public Book(Int32 id, String bookName):base(id)
		{
			Initialize();
			Name = bookName;
		}

		public Book(String bookName, IEnumerable<Author> authors, IEnumerable<Genre> genres, Boolean hasElectronic)
			:base(0)
		{
			Initialize(authors.Select(a=>new BookAuthor(this, a.Id)), genres.Select(g => new BookGenre(this, g.Id)));
			Name = bookName;
			HasElectronic = hasElectronic;
		}

		public Book(String bookName, IEnumerable<Author> authors = null, Genre genre=null, Boolean hasElectronic = false)
			: base(0)
		{
			if (authors != null)
				Initialize(authors.Select(a => new BookAuthor(this, a)));
			else
				Initialize();

			Name = bookName;
			if (genre != null)
				AddGenre(genre);
			HasElectronic = hasElectronic;
		}

		public Book(String bookName, Author author, Genre genre=null, Boolean hasElectronic = false)
			: base(0)
		{
			Initialize();
			Name = bookName;
			AddAuthor(author);
			if (genre != null)
				AddGenre(genre);
			HasElectronic = hasElectronic;
		}
		#endregion .ctor

		public void AddGenre(Genre genre)
		{
			BookGenres.Add(new BookGenre(this, genre));
		}

		public void AddGenre(Int32 genreId) 
			=> BookGenres.Add(new BookGenre(this, genreId));

		public void AddGenres(IEnumerable<Int32> genres)
		{
			foreach (var genreId in genres)
			{
				BookGenres.Add(new BookGenre(this, genreId));
			}
		}

		public void AddAuthor(Author author)
			=> BookAuthors.Add(new BookAuthor(this, author));

		public void AddAuthor(Int32 authorId)
			=> BookAuthors.Add(new BookAuthor(this, authorId));

		public void AddAuthors(IEnumerable<Author> authors)
		{
			foreach (var author in authors)
			{
				BookAuthors.Add(new BookAuthor(this, author));
			}  
		}

		public void AddAuthors(IEnumerable<Int32> authors)
		{
			foreach (var authorId in authors)
			{
				BookAuthors.Add(new BookAuthor(this, authorId));
			}
		}


		public override string ToString()
		{
			StringBuilder sb = new StringBuilder($"[{Id}]:\"{Name}\"");
			if (BookAuthors.Count() > 0)
			{
				sb.Append(": {");
				foreach (var bookAuthor in BookAuthors)
					sb.AppendFormat("[{0}]:{1},", bookAuthor.AuthorId, bookAuthor.Author != null ? bookAuthor.Author.Name : "not-loaded");
				sb.Remove(sb.Length - 1, 1);
				sb.Append("}, ");
			}
			else
				sb.Append(": {unkonwn author(s)}, ");

			sb.Append($"Raiting:{Raiting ?? 0}, ");
			//sb.Append($"Genre:{Genre}");
			return sb.ToString();
		}

		internal static class Rules
		{
			/// <summary>
			/// Правило, относящее книги к популярным
			/// </summary>
			public static readonly Rule<Book> PopularBook 
				= Rule<Book>.Create((b) => b.Raiting > 5);

			/// <summary>
			/// Правило, относящее книги к фантастическим
			/// </summary>
			public static readonly Rule<Book> FantasticBook  
				= Rule<Book>.Create( (b) => b.BookGenres.AsQueryable().Any(BookGenre.Rules.FantasticBookGenre) );

			/// <summary>
			/// Правило, относящее книгу к электронной
			/// </summary>
			public static Rule<Book> ElectronicBook =
				Rule<Book>.Create( (b) => b.HasElectronic == true );
		}

	}

}