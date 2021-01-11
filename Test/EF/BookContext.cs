using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;


namespace Data.Cqrs.Test.EF
{
	public class BookContext : DbContext
	{
		public virtual DbSet<Genre> Genres { get; set; }

		public virtual DbSet<Author> Authors { get; set; }

		public virtual DbSet<Publisher> Publishers { get; set; }

		public virtual DbSet<Book> Books { get; set; }

		public virtual DbSet<BookGenre> BookGenres { get; set; }

		public virtual DbSet<BookAuthor> BookAuthors { get; set; }

		public BookContext(): base("BookContext")
		{
			Database.SetInitializer(new BooksInitializer());
			Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			modelBuilder.Conventions.Remove<PluralizingEntitySetNameConvention>();
			
			//конфигурация сущностей
			modelBuilder.Configurations.Add(new AuthorMap());
			modelBuilder.Configurations.Add(new GenreMap());
			modelBuilder.Configurations.Add(new PublisherMap());
			modelBuilder.Configurations.Add(new BooksMap());
			modelBuilder.Configurations.Add(new BookGenreMap());
			modelBuilder.Configurations.Add(new BookAuthorMap());
		}

		private class AuthorMap: EntityTypeConfiguration<Author>
		{
			public AuthorMap():base()
			{
				HasKey(p => p.Id);
				Property(p => p.Name).IsRequired();
			}
		}

		private class GenreMap : EntityTypeConfiguration<Genre>
		{
			public GenreMap() : base()
			{
				HasKey(p => p.Id);
				Property(p => p.Name).IsRequired();
			}
		}

		private class PublisherMap : EntityTypeConfiguration<Publisher>
		{
			public PublisherMap() : base()
			{
				HasKey(p => p.Id);
				Property(p => p.Id).IsRequired().HasMaxLength(10);
				Property(p => p.Name).IsRequired();
			}
		}

		private class BooksMap : EntityTypeConfiguration<Book>
		{
			public BooksMap()
				: base()
			{
				// прямое указание ключевого поля. Также, для составного поля this.HasKey(t => new { t.Id, t.Name });
				HasKey(t => t.Id);
				Property(t => t.Name).IsRequired();

				// многие-ко-многим
				// с указанием таблицы связи и переопределенными ключами
				// EF плохо работает с распознаванием неизмененных записей, когда в качестве авторов есть список ключей.
				// Нужно вручную регистрировать в DbContext
				// Поэтому явно определяем BookAuthor
				/*
				HasMany(t => t.Authors)
					.WithMany(t => t.Books)
					.Map(m =>
					{
						m.MapLeftKey("BookId");
						m.MapRightKey("AuthorId");
						m.ToTable("BookAuthor");
					}
					);
				*/
				
					

				// один-ко-многим, с обязательным указанием значения (обязательное справочное поле)
				HasOptional(t => t.Publisher)
					.WithMany(p => p.Books)
					.HasForeignKey(t => t.PublisherId);

			}
		}

		private class BookGenreMap: EntityTypeConfiguration<BookGenre>
		{
			public BookGenreMap():base()
			{
				HasKey(t => new { t.BookId, t.GenreId });

				HasRequired(t => t.Book)
					.WithMany(b => b.BookGenres)
					.HasForeignKey(bg=>bg.BookId)
					.WillCascadeOnDelete();

				HasRequired(t => t.Genre)
					.WithMany(g => g.BookGenresCollection)
					.HasForeignKey(bg => bg.GenreId)
					.WillCascadeOnDelete();
			}
		}

		private class BookAuthorMap : EntityTypeConfiguration<BookAuthor>
		{
			public BookAuthorMap() : base()
			{
				Ignore(t => t.Id);

				HasKey(t => new { t.BookId, t.AuthorId });

				HasRequired(t => t.Book)
					.WithMany(b => b.BookAuthors)
					.HasForeignKey(ba => ba.BookId)
					.WillCascadeOnDelete();

				HasRequired(t => t.Author)
					.WithMany(a => a.BookAuthors)
					.HasForeignKey(ba => ba.AuthorId)
					.WillCascadeOnDelete();

			}
		}

	}

	public class BooksInitializer: DropCreateDatabaseIfModelChanges<BookContext>
	//public class BooksInitializer : CreateDatabaseIfNotExists<BookContext>
	//public class BooksInitializer : DropCreateDatabaseAlways<BookContext>
	{
		protected override void Seed(BookContext context)
		{
			base.Seed(context);

			context.Genres.Add(new Genre("<не указан>"));
			context.Genres.Add(new Genre("Учебная"));
			context.SaveChanges();

			Author author;
			author=context.Authors.Add(Author.Create("Толстой Л.Н."));
			Genre genre= context.Genres.Add(new Genre("Реализм"));

			Book book;

			book = new Book("Война и мир", author, genre);
			context.Books.Add(book);

			genre = context.Genres.Add(new Genre("Детская"));
			Author[] authors = new Author[]
			{
				context.Authors.Add(Author.Create("Крокодил Гена")),
				context.Authors.Add(Author.Create("Чебурашка"))
			};
			//context.SaveChanges();
			book = new Book("Приключения Крокодила Гены и Чебурашки (автобиография)", authors: authors, genre: genre);

			context.Books.Add(book);
			context.SaveChanges();

			genre = context.Genres.Add(new Genre("Фантастика"));
			context.Books.Add
				(
					new Book("Наш враг Шапокляк", authors, genre, true) { Raiting=10 }
				);

			context.Publishers.Add(new Publisher(id: "UNKNOWN", publisherName: "-неизвестно-"));
			context.Publishers.Add(new Publisher(id: "PITER", publisherName: "Питер"));
			context.Publishers.Add(new Publisher(id: "SHAPO", publisherName: "Шапокляк"));

			try
			{
				context.SaveChanges();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}