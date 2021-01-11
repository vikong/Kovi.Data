using System;
using System.ComponentModel.DataAnnotations;

namespace Kovi.Data.Cqrs
{
	public abstract class HasIdBase<TKey> : IHasId<TKey>
		where TKey : IComparable, IComparable<TKey>, IEquatable<TKey>
	{
		[Required]
		public virtual TKey Id { get; set; }

		Object IHasId.Id => Id;

		public HasIdBase()
		{ }

		public HasIdBase(TKey key)
		{
			Id = key;
		}
	}
}