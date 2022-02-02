using System;
using System.Collections.Generic;
using Kovi.Data.Cqrs;
using Kovi.Data.Dapper;

namespace Data.Cqrs.Test.Dapper
{
	/// <summary>
	/// Все книги
	/// </summary>
	public class DapperAllBookQriteria : IQriteria
	{ }

	/// <summary>
	/// Книга по Id
	/// </summary>
	public class DapperBookByIdQriteria : IQriteria
	{
		public Int32 Id { get; set; }
	}

	public sealed class BooksDapperQuery
		: BaseDapperQuery<DapperAllBookQriteria, IEnumerable<BookDto>>
	{
		public override String Sql
			=> @"SELECT * FROM [Book]";

		public BooksDapperQuery(IEnumQueryObjectHandler<BookDto> handler)
			: base(handler)
		{ }
	}

	public sealed class BookByIdDapperQuery : BaseDapperQuery<DapperBookByIdQriteria, BookDto>
	{
		public override String Sql
			=> @"SELECT * FROM [Book] WHERE [Id] = @Id";

		public BookByIdDapperQuery(ISingleQueryObjectHandler<BookDto> handler)
			: base(handler)
		{ }
	}
}