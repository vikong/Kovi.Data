using System;
using Kovi.Data.Cqrs;

namespace Data.Cqrs.Test.EF
{
	public class IdQriteria : IdQriteria<Int32>
	{ }

	public class LinqQriteria : IQriteria
	{ }

	public class NameQriteria : IQriteria
	{
		public String Name { get; set; }
	}

	/// <summary>
	/// Книги по автору или наименованию
	/// </summary>
	public class BookOrAuthorQriteria : NameQriteria
	{ }

}
