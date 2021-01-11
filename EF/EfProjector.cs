using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using Kovi.Data.Cqrs.Linq;
using Kovi.LinqExtensions;

namespace Kovi.Data.EF
{
	public interface INativeProjector : IProjector
	{ }

	public class EfProjector : IProjector, INativeProjector
	{
		public IQueryable<TReturn> Project<TReturn>(IQueryable queryable)
			=> queryable.Cast<TReturn>();

		public IEnumerable<TReturn> ProjectToEnum<TReturn>(IQueryable queryable)
			=> Project<TReturn>(queryable).AsEnumerable();

		public Task<IEnumerable<TReturn>> ProjectToEnumAsync<TReturn>(IQueryable queryable)
			=> Project<TReturn>(queryable)
				.ToListAsync()
				.ContinueWith(t => t.Result.AsEnumerable());
		public TReturn ProjectToSingle<TReturn>(IQueryable queryable)
			=> Project<TReturn>(queryable).SingleOrDefault();

		public Task<TReturn> ProjectToSingleAsync<TReturn>(IQueryable queryable)
			=> Project<TReturn>(queryable).SingleOrDefaultAsync();

		public IPage<TReturn> ProjectToPage<TEntity, TReturn>(IQueryable<TEntity> queryable, IPaging param)
			where TEntity : class
			where TReturn : class
		{
			if (!typeof(IOrderedQueryable).IsAssignableFrom(queryable.Expression.Type))
				throw new ArgumentException("Query must be ordered for pagination.", nameof(queryable));

			var totalCount = queryable.Count();
			if (totalCount == 0)
				return Page<TReturn>.Empty(param);

			return queryable
				.TakePage(param)
				.ProjectToEnum<TReturn>(this)
				.ToPage(totalCount, param);
		}

		public async Task<IPage<TReturn>> ProjectToPageAsync<TEntity, TReturn>(IQueryable<TEntity> queryable, IPaging param)
			where TEntity : class
			where TReturn : class
		{
			if (!typeof(IOrderedQueryable).IsAssignableFrom(queryable.Expression.Type))
				throw new ArgumentException("Query must be ordered for pagination.", nameof(queryable));

			var totalCount = await queryable.CountAsync();
			if (totalCount == 0)
				return Page<TReturn>.Empty(param);
			var result = await queryable.TakePage(param).ProjectToEnumAsync<TReturn>(this);
			return result.ToPage(totalCount, param);
		}

		public IPage<TReturn> ProjectToPage<TReturn>(IQueryable queryable, IPaging param)
		{
			throw new NotImplementedException();
		}

		public Task<IPage<TReturn>> ProjectToPageAsync<TReturn>(IQueryable queryable, IPaging param)
		{
			throw new NotImplementedException();
		}
	}

}
