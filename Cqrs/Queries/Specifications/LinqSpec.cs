using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Kovi.LinqExtensions;

namespace Kovi.Data.Cqrs.Linq
{
	public class DefaultLinqSpec<TEntity>
		: ILinqSpec<IQriteria, TEntity>
		where TEntity : class
	{
		public IQueryable<TEntity> Query(IQueryable<TEntity> query, IQriteria qrit)
			=> query;
	}

	public class FuncLinqSpec<TEntity, TParam>
		: ILinqSpec<TParam, TEntity>
		where TEntity : class
	{
		private Func<TParam, Expression<Func<TEntity, Boolean>>> Filter { get; }

		public IQueryable<TEntity> Query(IQueryable<TEntity> query, TParam qrit)
			=> query.Where(Filter.Invoke(qrit));

		public FuncLinqSpec(Func<TParam, Expression<Func<TEntity, Boolean>>> filterFunc)
		{
			Filter = filterFunc;
		}
	}

	public class ExpressionLinqSpec<TEntity, TParam>
		: ILinqSpec<TParam, TEntity>
		where TEntity : class
	{
		private readonly IFilterSpec<TParam, TEntity> _filterSpec;

		public virtual Expression<Func<TEntity, Boolean>> FilterExpr(TParam qrit)
			=> _filterSpec != null ? _filterSpec.Expression(qrit) : (t) => true;

		private readonly ISortingSpec<TEntity> _sortingSpec;

		public virtual IEnumerable<Sorting<TEntity, Object>> OrderBy(TParam qrit)
			=> _sortingSpec?.OrderBy(qrit);

		public IQueryable<TEntity> Query(IQueryable<TEntity> query, TParam qrit)
		{
			var filteredQuery = query
				.Where(FilterExpr(qrit));

			var order = OrderBy(qrit);

			return order != null && order.Any()
				? filteredQuery.OrderBy(order)
				: filteredQuery;
		}

		#region ctor

		public ExpressionLinqSpec()
		{ }

		public ExpressionLinqSpec(
			IFilterSpec<TParam, TEntity> filterSpec,
			ISortingSpec<TEntity> sortingSpec
			)
		{
			_filterSpec = filterSpec;
			_sortingSpec = sortingSpec;
		}

		public ExpressionLinqSpec(
			IFilterSpec<TParam, TEntity> filterSpec
			)
		{
			_filterSpec = filterSpec;
			_sortingSpec = null;
		}

		public ExpressionLinqSpec(
			Expression<Func<TEntity, Boolean>> filterExpression
			) : this(new ExpressionFilterSpec<TParam, TEntity>(filterExpression))
		{ }

		#endregion ctor
	}

	public static class LinqSpec<TEntity>
		where TEntity : class
	{
		public static ILinqSpec<IQriteria, TEntity> All()
			=> new DefaultLinqSpec<TEntity>();

		public static ILinqSpec<TParam, TEntity> Filter<TParam>(Expression<Func<TEntity, Boolean>> filterExpression)
			where TParam : IQriteria
			=> new ExpressionLinqSpec<TEntity, TParam>(filterExpression);

		public static ILinqSpec<TParam, TEntity> Filter<TParam>(IFilterSpec<TParam, TEntity> filterSpec)
			where TParam : IQriteria
			=> new ExpressionLinqSpec<TEntity, TParam>(filterSpec);

		public static ILinqSpec<TParam, TEntity> FilterFunc<TParam>(Func<TParam, Expression<Func<TEntity, Boolean>>> filterFunc)
			where TParam : IQriteria
			=> new FuncLinqSpec<TEntity, TParam>(filterFunc);

		public static ILinqSpec<TParam, TEntity> ByConvention<TParam>(TParam param)
			where TParam : IQriteria
			=> new ConventionFilterSpec<TEntity, TParam>();
	}

	public static class LinqSpecExtensions
	{
		public static IQueryable<TEntity> Apply<TParam, TEntity>(this IQueryable<TEntity> query, ILinqSpec<TParam, TEntity> spec, TParam qrit)
			where TEntity : class
			=> spec.Query(query, qrit);

		public static IQueryable Apply<TParam>(this ILinqProvider linqProvider, ILinqSpec<TParam> spec, TParam qrit)
			=> spec.Query(linqProvider, qrit);

	}
}