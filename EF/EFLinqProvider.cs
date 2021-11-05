using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

using Kovi.Data.Cqrs;

namespace Kovi.Data.EF
{
	public class EFLinqProvider : ILinqProvider
	{
		protected DbContext DbContext { get; private set; }

		public EFLinqProvider(DbContext context)
		{
			DbContext = context ?? throw new ArgumentNullException("Context is NULL.");
		}

		public override string ToString()
			=> $"EFLinq ConnectionString: [{DbContext.Database.Connection.ConnectionString}]";

		#region Члены ILinqProvider

		public IQueryable<TEntity> Query<TEntity>()
			where TEntity : class
			=> this.DbContext.Set<TEntity>();

		public IQueryable<TEntity> Query<TEntity>(String include)
			where TEntity : class
			=> this.DbContext.Set<TEntity>().Include(include);
		public IQueryable<TEntity> Query<TEntity>(IEnumerable<String> includes)
			where TEntity : class
		{
			var q = this.DbContext.Set<TEntity>() as DbQuery<TEntity>;
			foreach (var s in includes)
				q = q.Include(s);
			return q;
		}

		public IQueryable Query(Type entityType)
			=> this.DbContext.Set(entityType);
		#endregion

		//public IQueryable<TEntity> Set<TEntity>(String includePath)
		//	where TEntity : class
		//{
		//	IQueryable<TEntity> result=DbContext
		//		.Set<TEntity>()
		//		.Include(includePath);

		//	return result;
		//}

		//public IQueryable<TEntity> Set<TEntity>(Expression<Func<TEntity, Object>> Include)
		//	where TEntity : class
		//{
		//	IQueryable<TEntity> result=DbContext
		//		.Set<TEntity>()
		//		.Include(Include);

		//	return result;
		//}

		//public IQueryable<TEntity> Set<TEntity>(IEnumerable<Expression<Func<TEntity, Object>>> Includes)
		//	where TEntity: class
		//{
		//	IQueryable<TEntity> result=DbContext.Set<TEntity>() as IQueryable<TEntity>;
		//	foreach (Expression<Func<TEntity, Object>> ex in Includes)
		//		result=result.Include(ex);
		//	return result;
		//}


		#region IDisposable

		private bool _disposed = false;

		~EFLinqProvider()
			=> Dispose(false);

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			if (disposing)
			{
				// dispose managed state (managed objects).
				DbContext?.Dispose();
			}

			// free unmanaged resources (unmanaged objects) and override a finalizer below.
			// set large fields to null.
			_disposed = true;
		}

		#endregion IDisposable
	}
}
