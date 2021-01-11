using System;

using Kovi.Data.Cqrs;

namespace Kovi.Data.EF
{
	public class EFLinqProviderFactory : ILinqProviderFactory
	{
		protected IDbContextFactory ContextFactory { get; }

		public EFLinqProviderFactory(IDbContextFactory contextFactory)
		{
			ContextFactory = contextFactory ?? throw new ArgumentNullException("Context Factory is NULL");
		}

		public ILinqProvider Create(string connection = null, string context = null)
		{
			var ctx = ContextFactory.Create(connection, context);
			return new EFLinqProvider(ctx);
		}
	}
}
