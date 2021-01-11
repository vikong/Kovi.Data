using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Kovi.LinqExtensions;

namespace Kovi.Data.Cqrs.Linq
{
	/// <summary>
	/// Представляет функциональные возможности 
	/// конвертирования IQueryable в конечный результат запроса.
	/// </summary>
	public interface IProjector
	{
		IQueryable<TReturn> Project<TReturn>(IQueryable queryable);

		TReturn ProjectToSingle<TReturn>(IQueryable queryable);

		Task<TReturn> ProjectToSingleAsync<TReturn>(IQueryable queryable);

		IEnumerable<TReturn> ProjectToEnum<TReturn>(IQueryable queryable);

		Task<IEnumerable<TReturn>> ProjectToEnumAsync<TReturn>(IQueryable queryable);

		IPage<TReturn> ProjectToPage<TReturn>(IQueryable queryable, IPaging param);
		Task<IPage<TReturn>> ProjectToPageAsync<TReturn>(IQueryable queryable, IPaging param);

		IPage<TReturn> ProjectToPage<TEntity,TReturn>(IQueryable<TEntity> queryable, IPaging param)
			where TEntity : class
			where TReturn : class;

		Task<IPage<TReturn>> ProjectToPageAsync<TEntity, TReturn>(IQueryable<TEntity> queryable, IPaging param)
			where TEntity : class
			where TReturn : class;
	}

	public static class ProjectorExtensions
	{
		public static IQueryable<T> Project<T>(this IQueryable queryable, IProjector projector)
			=> projector.Project<T>(queryable);

		public static T ProjectToSingle<T>(this IQueryable queryable, IProjector projector)
			=> projector.ProjectToSingle<T>(queryable);

		public static Task<T> ProjectToSingleAsync<T>(this IQueryable queryable, IProjector projector)
			=> projector.ProjectToSingleAsync<T>(queryable);

		public static IEnumerable<T> ProjectToEnum<T>(this IQueryable queryable, IProjector projector)
			=> projector.ProjectToEnum<T>(queryable);

		public static Task<IEnumerable<T>> ProjectToEnumAsync<T>(this IQueryable queryable, IProjector projector)
			=> projector.ProjectToEnumAsync<T>(queryable);
	}
}