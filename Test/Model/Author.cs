using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Kovi.Data.Cqrs;
using Kovi.LinqExtensions.Specification;

namespace Data.Cqrs.Test
{
	/// <summary>
	/// Автор
	/// </summary>
	public class Author : BaseEntity
	{
		private String _name;
		public String Name
		{
			get { return _name; }
			set { _name = value ?? throw new ArgumentNullException("Author name"); }
		}

		public virtual ICollection<Book> Books { get; protected set; }

		public virtual ICollection<BookAuthor> BookAuthors { get; protected set; }

		private void Initialize()
		{
			Books = new List<Book>();
			BookAuthors = new List<BookAuthor>();
		}

		[Obsolete("Only for model binders and EF, don't use it in your code", true)]
		public Author ()
		{
			Initialize();
		}

		public Author(Int32 id) : base(id) {}

		public Author (Int32 id, String authorName):base(id)
		{
			Name=authorName;
			Books=new List<Book>();
		}

		public static Author Create(String authorName)
			=> new Author(0, authorName);

		/// <summary>
		/// Правило "Заслуженный писатель"
		/// </summary>
		public static readonly Rule<Author> HonoredAuthorRule 
			= Rule<Author>.Create( (a) => a.Books.Count>5 );

		public override string ToString() 
			=> $"Author: {Name}";
	}


	public class BookAuthor: HasIdBase<String>
	{

		public Int32 BookId { get; protected set; }
		public virtual Book Book { get; protected set; }

		public Int32 AuthorId { get; protected set; }
		public virtual Author Author { get; protected set; }

		public override String Id => BookId.ToString()+"-"+AuthorId.ToString();

		[Obsolete("Only for model binders and EF, don't use it in your code", true)]
		public BookAuthor()
		{ }

		public BookAuthor(Book book, Author author)
		{
			Book = book;
			BookId = book.Id;

			Author = author;
			AuthorId = author.Id;
		}

		public BookAuthor(Book book, Int32 authorId)
		{
			Book = book;
			BookId = book.Id;

			AuthorId = authorId;
		}
	}
}