using System;
using System.Collections.Generic;
using System.Linq;

namespace Kovi.Data.Cqrs
{
	/// <summary>
	/// Представляет интерфейс для получения Query{T}
	/// </summary>
	public interface ILinqProvider : IDisposable
	{
		/// <summary>
		/// Query object
		/// </summary>
		/// <typeparam name="TEntity">сущность</typeparam>
		/// <returns>Query object для TEntity</returns>
		IQueryable<TEntity> Query<TEntity>()
			where TEntity : class;



		/// <summary>
		/// Query object, with eagerly loading
		/// </summary>
		/// <typeparam name="TEntity">entity</typeparam>
		/// <param name="include">имя сущности для упреждающей загрузки.</param>
		/// <returns>Query object for TEntity</returns>
		IQueryable<TEntity> Query<TEntity>(String include)
			where TEntity : class;

		IQueryable<TEntity> Query<TEntity>(IEnumerable<String> includes)
			where TEntity : class;

		/// <summary>
		/// Query object
		/// </summary>
		/// <param name="entityType">Type of entity</param>
		/// <returns>Query object for entity</returns>
		IQueryable Query(Type entityType);
	}
}