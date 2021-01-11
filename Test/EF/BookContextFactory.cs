using System.Data.Entity;

using Kovi.Data.EF;

namespace Data.Cqrs.Test.EF
{
	public class BookContextFactory : IDbContextFactory
	{
		public DbContext Create(string connection = null, string context = null) 
			=> new BookContext();
	}
}
