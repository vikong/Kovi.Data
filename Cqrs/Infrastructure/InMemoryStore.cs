using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Kovi.Data.Cqrs.Linq
{
	/// <summary>
	/// Класс, представляющий эмуляцию ILinqProvider, IUnitOfWork в оперативной памяти
	/// </summary>
	/// <seealso cref="Kovi.Data.Cqrs.ILinqProvider" />
	/// <seealso cref="Kovi.Data.Cqrs.IUnitOfWork" />
	public class InMemoryStore
		: ILinqProvider, IUnitOfWork, IDisposable
	{
		private readonly ConcurrentDictionary<Type, ConcurrentDictionary<String, IHasId>> _store
			= new ConcurrentDictionary<Type, ConcurrentDictionary<String, IHasId>>();

		private ConcurrentDictionary<String, IHasId> GetStore(Type t)
		{
			if (_store.TryGetValue(t, out ConcurrentDictionary<String, IHasId> result))
				return result;
			throw new InvalidOperationException($"Type [{t.FullName}] has not been registered in the store. Register it before use.");
		}

		private ConcurrentDictionary<String, IHasId> GetStore<T>()
			=> GetStore(typeof(T));

		public void Register<TEntity>()
			where TEntity : class, IHasId
			=> Register(typeof(TEntity));

		public void Register(Type type)
		{
			if (!typeof(IHasId).IsAssignableFrom(type))
				throw new InvalidOperationException($"Type {type.FullName} must implement 'IHasId' interface.");

			_store.TryAdd(type, new ConcurrentDictionary<String, IHasId>());
		}

		public IQueryable<TEntity> Query<TEntity>()
			where TEntity : class
			=> GetStore<TEntity>().Select(e => e.Value).Cast<TEntity>().AsQueryable();

		public IQueryable<TEntity> Query<TEntity>(String include)
			where TEntity : class
			=> Query<TEntity>();

		public IQueryable<TEntity> Query<TEntity>(IEnumerable<String> includes)
			where TEntity : class
			=> Query<TEntity>();

		public IQueryable Query(Type t)
			=> GetStore(t).Select(e => e.Value).AsQueryable();

		public TEntity Find<TEntity>(Object id)
			where TEntity : class, IHasId
		{
			if (GetStore<TEntity>().TryGetValue(id.ToString(), out IHasId value))
				return (TEntity)value;
			else
				return default;
		}

		public IHasId Find(Type entityType, Object id)
		{
			GetStore(entityType).TryGetValue(id.ToString(), out IHasId value);
			return value;
		}

		void IUnitOfWork.Update<TEntity>(TEntity entity)
		{
			GetStore<TEntity>().AddOrUpdate(entity.Id.ToString(), entity, (key, val) => val = entity);
		}

		void IUnitOfWork.Add<TEntity>(TEntity entity)
		{
			if (!GetStore<TEntity>().TryAdd(entity.Id.ToString(), entity))
				throw new InvalidOperationException("Duplicate key.");
		}

		void IUnitOfWork.Delete<TEntity>(TEntity entity)
			=> GetStore<TEntity>().TryRemove(entity.Id.ToString(), out _);

		public ILinqProvider Linq => this;

		public void Commit()
		{
		}

		public void Rollback()
		{
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(Boolean disposing)
		{
		}
	}
}