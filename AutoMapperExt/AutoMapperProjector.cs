using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;

using AutoMapper;

using Kovi.LinqExtensions;
using Kovi.Data.Cqrs.Linq;

namespace Kovi.Data.Automapper
{

	public class AutoMapperProjector 
		: IProjector
	{
		protected readonly IMapper _mapper;
		protected IConfigurationProvider ConfigurationProvider
			=> _mapper.ConfigurationProvider;

		public AutoMapperProjector(IMapper mapper)
		{
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		public IQueryable<TReturn> Project<TReturn>(IQueryable queryable)
		{
			var res = queryable.ProjectToQueryable<TReturn>(ConfigurationProvider);
			return res;
		}

		public TReturn ProjectToSingle<TReturn>(IQueryable queryable)
			=> queryable.ProjectToSingleOrDefault<TReturn>(ConfigurationProvider);

		public Task<TReturn> ProjectToSingleAsync<TReturn>(IQueryable queryable)
			=> queryable.ProjectToSingleOrDefaultAsync<TReturn>(ConfigurationProvider);

		public IEnumerable<TReturn> ProjectToEnum<TReturn>(IQueryable queryable)
			=> queryable
				.ProjectToList<TReturn>(ConfigurationProvider)
				.AsEnumerable();

		public Task<IEnumerable<TReturn>> ProjectToEnumAsync<TReturn>(IQueryable queryable)
		{
			return queryable
				.ProjectToListAsync<TReturn>(ConfigurationProvider)
				.ContinueWith(t => t.Result.AsEnumerable());
		}

		public IPage<TReturn> ProjectToPage<TEntity, TReturn>(IQueryable<TEntity> query, IPaging param)
			where TEntity : class
			where TReturn : class
		{
			if (!typeof(IOrderedQueryable).IsAssignableFrom(query.Expression.Type))
				throw new ArgumentException("Query must be ordered for pagination.", nameof(query));
			//Queryable.Count((dynamic)query);
			
			var totalCount = query.Count();
			if (totalCount == 0)
				return Page<TReturn>.Empty(param);

			return query
				.TakePage(param)
				.ProjectToEnum<TReturn>(this)
				.ToPage(totalCount, param);

		}

		public async Task<IPage<TReturn>> ProjectToPageAsync<TEntity, TReturn>(IQueryable<TEntity> query, IPaging param)
			where TEntity : class
			where TReturn : class
		{
			if (!typeof(IOrderedQueryable).IsAssignableFrom(query.Expression.Type))
				throw new ArgumentException("Query must be ordered for pagination.", nameof(query));
			var totalCount = await query.CountAsync();
			if (totalCount == 0)
				return Page<TReturn>.Empty(param);
			var result = await query.TakePage(param).ProjectToEnumAsync<TReturn>(this);
			return result.ToPage(totalCount, param);
		}

		public IPage<TReturn> ProjectToPage<TReturn>(IQueryable queryable, IPaging param)
		{
			var totalCount = queryable.Count();
			if (totalCount == 0)
				return Page<TReturn>.Empty(param);
			return queryable
				.TakePage(param.PageNo, param.PageSize)
				.ProjectToEnum<TReturn>(this)
				.ToPage(totalCount, param);
		}

		public async Task<IPage<TReturn>> ProjectToPageAsync<TReturn>(IQueryable queryable, IPaging param)
		{
			var totalCount = queryable.Count();
			if (totalCount == 0)
				return Page<TReturn>.Empty(param);
			var result = await queryable
				.TakePage(param.PageNo, param.PageSize)
				.ProjectToEnumAsync<TReturn>(this);
			return result.ToPage(totalCount, param);
		}
	}

}
