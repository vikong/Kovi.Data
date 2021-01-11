using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Kovi.Data.Cqrs;
using Kovi.Data.Cqrs.Linq;
using Kovi.LinqExtensions.Specification;
using System.Threading.Tasks;

namespace Data.Cqrs.Test.EF
{
	[CreateBookCmdValidate]
	public sealed class CreateBookCmd: ICmdParam, IQriteria
	{
		[Required(AllowEmptyStrings = false, ErrorMessage ="Требуется наименование")]
		public String Name { get; set; }

		public ICollection<Int32> Genres { get; set; }

		public Boolean HasElectronic { get; set; }

		[Range(1000, 3000, ErrorMessage ="Год издательства некорректен")]
		public Int32 Published { get; set; }

		public ICollection<Int32> Authors { get; set; }
	}

	public sealed class CreateBookCmdValidateAttribute
		: ValidateAttribute
	{
		public override IEnumerable<ValidationResult> Validate(Object context)
		{
			CreateBookCmd param = (CreateBookCmd)context;

			if (param.Published > DateTime.Now.Year)
				yield return new ValidationResult(Messages.BookPublishYear, new String[] { nameof(param.Published) });

			if (param.Published>2020 && !param.HasElectronic)
				yield return new ValidationResult(Messages.MustHaveElectronic, new String[] { nameof(param.Published), nameof(param.HasElectronic) });

			yield break;
		}
	}

	/// <summary>
	/// Проверяет на наличие дубликатов книги по названию
	/// </summary>
	public class CreateBookCheckQuery 
		: RequestQuery<CreateBookCmd, Result>
	{
		public CreateBookCheckQuery(ILinqRequestHandler handler) 
			: base(handler)
		{}

		public override Result Request(ILinqProvider linq, CreateBookCmd qrit)
		{
			var query = linq.Query<Book>()
				.Where(b => b.Name == qrit.Name)
				.AsNoTracking();

			if (!query.Any())
				return Result.Ok();

			return new Failure(Messages.BookNameUnique, query.ToList());
		}
	}

	[ValidateCommand(typeof(CreateBookCheckQuery))]
	public sealed class CreateBookCommand 
		: LinqCommand<CreateBookCmd>
		, ICommand<CreateBookCmd>
	{
		public CreateBookCommand(ILinqCommandHandler commandHandler) : base(commandHandler){}

		public override Result Execute(IUnitOfWork uow)
		{

			Book newBook = new Book(Cmd.Name, hasElectronic: Cmd.HasElectronic);

			if (Cmd.Authors != null)
				newBook.AddAuthors(Cmd.Authors);

			if (Cmd.Genres != null)
				newBook.AddGenres(Cmd.Genres);

			uow.Add(newBook);

			uow.Commit();

			return Result.Ok(newBook);
		}
	}

	public class BookChange 
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int GenreId { get; set; }
		public Boolean? HasElectronic { get; set; }
		public int Published { get; set; }

		public BookChange(Book book)
		{
			Id=book.Id;
			Name=book.Name;
			//GenreId=book.GenreId;
			HasElectronic=book.HasElectronic;
			Published=book.Published;
		}
	}
}
