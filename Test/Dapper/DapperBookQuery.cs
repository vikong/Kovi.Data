using System;
using System.Collections;
using System.Collections.Generic;

using Kovi.Data.Cqrs;
using Kovi.Data.Dapper;

namespace Data.Cqrs.Test.Dapper
{
	/// <summary>
	/// Все книги
	/// </summary>
	public class AllBookQriteria : IQriteria { }

	public class AllBookQuery
		: DapperQuery<AllBookQriteria, IEnumerable<BookDto>>
		, IQuery<AllBookQriteria, IEnumerable<BookDto>>
	{
		public static readonly String Sql = @"SELECT * FROM [Book] ";

		public override QueryObject Query(AllBookQriteria param = null)
			=> new QueryObject(Sql);

		public AllBookQuery(IEnumDapperHandler<BookDto> dapperHandler) : base(dapperHandler) { }
	}

	/// <summary>
	/// Книга по Id
	/// </summary>
	public class BookByIdQriteria : IQriteria
	{
		public Int32 Id { get; set; }
	}

	public class BookByIdQuery : DapperQuery<BookByIdQriteria, BookDto>
		, IQuery<BookByIdQriteria,BookDto>
	{

		public override QueryObject Query(BookByIdQriteria param)
			=> new QueryObject(AllBookQuery.Sql + "WHERE [Id] = @Id ", param);

		public BookByIdQuery(ISingleDapperHandler<BookDto> dapperHandler) : base(dapperHandler) { }

	}

}
