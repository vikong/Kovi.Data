using System;

namespace Kovi.Data.Cqrs
{
	public class EmptyQriteria : IQriteria
	{ }

	public class IdQriteria<TKey>
		: IHasId<TKey>
		, IQriteria
	where TKey : IComparable<TKey>, IEquatable<TKey>
	{
		public TKey Id { get; set; }

		Object IHasId.Id => Id;
	}

	public class PageQriteria<T> : IPageQriteria<T>
	{
		public T Subject { get; set; }

		public String OrderBy { get; set; }

		public Int32 PageNo { get; set; }

		public Int32 PageSize { get; set; }
	}
}