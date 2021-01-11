using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;

using Kovi.Data.Cqrs;

namespace Kovi.Data.EF
{
	public class EFUnitOfWork : IUnitOfWork
	{
		private readonly DbContext _dbContext;
		protected DbContext DataContext { get { return _dbContext; } }

		public EFUnitOfWork(DbContext dbContext)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}

		protected IDbSet<TEntity> Set<TEntity>()
			where TEntity : class
			=> DataContext.Set<TEntity>();


		#region Члены IUnitOfWork

		void IUnitOfWork.Commit()
		{
			try
			{
				DataContext.SaveChanges();
			}
			catch (DbEntityValidationException ex)
			{
				StringBuilder sb = new StringBuilder("Ошибка проверки данных");
				foreach (var e in ex.EntityValidationErrors)
					foreach (var err in e.ValidationErrors)
						sb.AppendFormat("\r\n{0}->{1}: {2}", e.Entry.Entity.GetType().FullName, err.PropertyName, err.ErrorMessage);

				throw BuildException(sb.ToString());
			}

			catch (DBConcurrencyException ex)
			{
				throw BuildException("Не удалось сохранить изменения, так как запись была изменена другим оператором. Повторите попытку.", ex);
			}

			catch (DbUpdateException ex)
			{
				throw new DataException("Не удалось записать изменения. Повторите попытку. Если ошибка будет повторяться, сообщите в техподдержку.", ex);
			}

		}

		void IUnitOfWork.Rollback()
		{
			DataContext
				.ChangeTracker
				.Entries()
				.ToList()
				.ForEach(e => e.Reload());
		}

		void IUnitOfWork.Add<TEntity>(TEntity entity)
		{
			Set<TEntity>().Add(entity);

			//var entries = DataContext.ChangeTracker.Entries();
			//DbEntityEntry dbEntityEntry = DataContext.Entry(entity);
			//if (dbEntityEntry.State != EntityState.Detached)
			//{
			//	dbEntityEntry.State = EntityState.Added;
			//}
			//else
			//{
			//	DataContext.Set<TEntity>().Add(entity);
			//}
		}

		void IUnitOfWork.Update<TEntity>(TEntity entity)
		{
			DbEntityEntry dbEntityEntry = DataContext.Entry(entity);
			if (dbEntityEntry.State == EntityState.Detached)
				DataContext.Set<TEntity>().Attach(entity);

			dbEntityEntry.State = EntityState.Modified;
		}

		void IUnitOfWork.Delete<TEntity>(TEntity entity)
		{
			Set<TEntity>().Remove(entity);
			//DbEntityEntry dbEntityEntry = DataContext.Entry(entity);
			//dbEntityEntry.State=EntityState.Deleted;
		}

		public ILinqProvider Linq
			=> new EFLinqProvider(_dbContext);

		#endregion

		private Exception BuildException(string message, Exception ex = null)
		{
			return new DataException(
				string.Format("Ошибка при изменении данных {0}\r\n{1}",
				message,
				ex));
		}

		#region Члены IDisposable

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
				if (_dbContext != null) _dbContext.Dispose();
		}

		#endregion

	}
}
