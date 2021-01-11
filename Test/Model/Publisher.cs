using System;
using System.Collections.Generic;
using Kovi.Data.Cqrs;

namespace Data.Cqrs.Test
{
	/// <summary>
	/// Издатель
	/// </summary>
	public class Publisher : IHasId<String>
	{
		public String Id { get; set; }

		object IHasId.Id => Id;

		private String _name;
		public String Name
		{
			get { return _name; }
			set { _name = value ?? throw new ArgumentNullException("Publisher name"); }
		}

		public virtual ICollection<Book> Books { get; protected set; }

		[Obsolete("Only for model binders and EF, don't use it in your code", true)]
		internal Publisher()
		{
			Initialize();
		}

		public Publisher(String id, String publisherName)
		{
			Id = id;
			Name = publisherName;
			Initialize();
		}

		public override string ToString() 
			=> $"Publisher:({Id}){Name}";

		private void Initialize() { Books = new HashSet<Book>(); }
	}

}