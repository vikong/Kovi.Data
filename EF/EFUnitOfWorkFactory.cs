using System;

using Kovi.Data.Cqrs;

namespace Kovi.Data.EF
{
	public class EFUnitOfWorkFactory : IUnitOfWorkFactory
	{
		protected IDbContextFactory ContextFactory { get; }

		public EFUnitOfWorkFactory(IDbContextFactory contextFactory)
		{
			ContextFactory = contextFactory ?? throw new ArgumentNullException("Null ContextFactory");
		}


		#region Члены IUnitOfWorkFactory

		public IUnitOfWork Create(string connection, string context)
			=> new EFUnitOfWork(ContextFactory.Create(connection, context));

		public IUnitOfWork Create(string connection)
			=> new EFUnitOfWork(ContextFactory.Create(connection));

		public IUnitOfWork Create()
			=> new EFUnitOfWork(ContextFactory.Create());


		#endregion
	}
}
