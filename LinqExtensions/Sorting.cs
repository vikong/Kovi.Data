using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

/// <summary>
/// https://github.com/hightechgroup/costeffectivecode/tree/master/src/CostEffectiveCode/Ddd/Pagination
/// </summary>
namespace Kovi.LinqExtensions
{

	public enum SortOrder
	{
		Asc = 1,
		Desc = 2
	}

	public interface ISortingSpec<TEntity>
		where TEntity: class
	{
		IEnumerable< Sorting<TEntity, Object> > OrderBy<TParam>(TParam param);
	}


	public class Sorting<TEntity, TKey> 
		where TEntity : class
	{
		public Expression<Func<TEntity, TKey>> Expression { get; private set; }

		public SortOrder SortOrder { get; private set; }

		public Sorting(Expression<Func<TEntity, TKey>> expression, SortOrder sortOrder = SortOrder.Asc)
		{
			Expression = expression ?? throw new ArgumentNullException(nameof(expression));
			SortOrder = sortOrder;
		}

		public static implicit operator Expression<Func<TEntity, TKey>>(Sorting<TEntity, TKey> sorter)
			=> sorter.Expression;

	}

	public static class Sorting<TEntity>
		where TEntity : class
	{
		public static ISortingSpec<TEntity> None => new SimpleSortingSpec<TEntity>();
	}

	public class SimpleSortingSpec<TEntity>
		: ISortingSpec<TEntity>
		where TEntity : class
	{
		private IEnumerable<Sorting<TEntity, Object>> Order { get; }

		public IEnumerable<Sorting<TEntity, Object>> OrderBy<TParam>(TParam param) 
			=> Order;

		public SimpleSortingSpec()
			:this(Enumerable.Empty<Sorting<TEntity, Object>>())
		{ }

		public SimpleSortingSpec(IEnumerable<Sorting<TEntity, Object>> order)
		{
			Order = order ?? throw new ArgumentNullException(nameof(order));
		}

		public SimpleSortingSpec(params Expression<Func<TEntity, Object>>[] orderExpressions)
		{
			Order = new List<Sorting<TEntity, Object>>(orderExpressions.Select(e => new Sorting<TEntity, Object>(e)));
		}

	}


	public static class SortingHelper
	{
		public static IOrderedQueryable<TEntity> OrderBy<TEntity, TKey>(this IQueryable<TEntity> queryable
			, Sorting<TEntity, TKey> sorting)
			where TEntity : class
			=> sorting.SortOrder == SortOrder.Asc
				? queryable.OrderBy(sorting.Expression)
				: queryable.OrderByDescending(sorting.Expression);

		public static IOrderedQueryable<TEntity> ThenBy<TEntity, TKey>(this IOrderedQueryable<TEntity> queryable
			, Sorting<TEntity, TKey> sorting)
			where TEntity : class
			=> sorting.SortOrder == SortOrder.Asc
				? queryable.ThenBy(sorting.Expression)
				: queryable.ThenByDescending(sorting.Expression);

		public static IOrderedQueryable<TEntity> OrderBy<TEntity, TKey>(this IQueryable<TEntity> queryable
			, IEnumerable< Sorting<TEntity,TKey> > sorting)
			where TEntity : class
		{
			if (sorting==null || !sorting.Any())
			{
				throw new ArgumentException("OrderBy can't be null or empty", nameof(sorting));
			}
			var ordered = queryable.OrderBy(sorting.First());
			return sorting.Skip(1).Aggregate(ordered, (current, order) => current.ThenBy(order));
		}


	}
}
