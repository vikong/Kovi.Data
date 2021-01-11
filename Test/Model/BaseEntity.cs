using System;

using Kovi.Data.Cqrs;

namespace Data.Cqrs.Test
{
	public class BaseEntity
		: IHasId<Int32>
	{
		public Int32 Id { get; protected set; }

		Object IHasId.Id => Id;

		[Obsolete("Only for model binders and EF, don't use it in your code", true)]
		internal BaseEntity()
		{ }

		public BaseEntity(Int32 id) 
			=> Id = id;
	}

}