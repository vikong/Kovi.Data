using Kovi.Data.Cqrs;

namespace Kovi.Data.EF
{
	public class EFDataProviderFactory : IDataProviderFactory
	{
		private readonly IDbContextFactory _contextFactory;
		public EFDataProviderFactory(IDbContextFactory contextFactory)
		{
			_contextFactory = contextFactory;
		}

		public ILinqProviderFactory LinqFactory
			=> new EFLinqProviderFactory(_contextFactory);

		public IUnitOfWorkFactory UowFactory
			=> new EFUnitOfWorkFactory(_contextFactory);

	}

}
