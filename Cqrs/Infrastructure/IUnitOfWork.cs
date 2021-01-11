using System;

namespace Kovi.Data.Cqrs
{
	public interface IUnitOfWork : IDisposable
	{
		/// <summary>
		/// Подтверждение изменений
		/// </summary>
		void Commit();

		/// <summary>
		/// Откат изменений
		/// </summary>
		void Rollback();

		/// <summary>
		/// Добавление сущности
		/// </summary>
		/// <typeparam name="TEntity">тип сущности</typeparam>
		/// <param name="entity">сущность</param>
		void Add<TEntity>(TEntity entity)
			where TEntity : class, IHasId;

		/// <summary>
		/// Изменение сущности
		/// </summary>
		/// <typeparam name="TEntity">тип сущности</typeparam>
		/// <param name="entity">сущность</param>
		void Update<TEntity>(TEntity entity)
			where TEntity : class, IHasId;

		/// <summary>
		/// Удаление сущности
		/// </summary>
		/// <typeparam name="TEntity">тип сущности</typeparam>
		/// <param name="entity">сущность</param>
		void Delete<TEntity>(TEntity entity)
			where TEntity : class, IHasId;

		ILinqProvider Linq { get; }
	}
}