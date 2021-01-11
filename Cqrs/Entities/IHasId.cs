using System;

namespace Kovi.Data.Cqrs
{
	public interface IHasId : IQriteria
	{
		Object Id { get; }
	}

	public interface IHasId<TKey> : IHasId
		where TKey : IComparable<TKey>, IEquatable<TKey>
	{
		new TKey Id { get; }
	}

	public static class HasId
	{
		public static Boolean IsNew<TKey>(this IHasId<TKey> obj)
			where TKey : IComparable<TKey>, IEquatable<TKey>
		{
			return obj.Id == null || obj.Id.Equals(default(TKey));
		}
	}
}