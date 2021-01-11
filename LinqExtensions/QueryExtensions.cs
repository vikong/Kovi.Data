using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Kovi.LinqExtensions.Specification;

namespace Kovi.LinqExtensions
{
	public static class QueryExtensions
	{
		public static Int32 Count(this IQueryable source)
		{
			var expression = Expression.Call(
				typeof(Queryable), nameof(Queryable.Count), new[] { source.ElementType },
				source.Expression);
			return source.Provider.Execute<Int32>(expression);
		}

		public static IQueryable TakePage(this IQueryable source, Int32 page, Int32 pageSize)
		{
			if (pageSize <= 0)
				return source;
			
			MethodCallExpression expression;
			expression = Expression.Call(
				typeof(Queryable), nameof(Queryable.Skip), new[] { source.ElementType },
				source.Expression, 
				Expression.Constant((Math.Max(page, 1)-1)*pageSize));
			IQueryable query = source.Provider.CreateQuery(expression);
			
			expression = Expression.Call(
				typeof(Queryable), nameof(Queryable.Take), new[] { source.ElementType},
				query.Expression,
				Expression.Constant(pageSize)
				);
			return query.Provider.CreateQuery(expression);
		}

		/// <summary>
		/// представляет постраничный запрос
		/// </summary>
		/// <typeparam name="TSource">тип данных в источнике</typeparam>
		/// <param name="source">источник данных</param>
		/// <param name="page">номер страницы</param>
		/// <param name="pageSize">страница</param>
		/// <returns>запрос</returns>
		public static IQueryable<TSource> Paged<TSource>(
				this IOrderedQueryable<TSource> source, Int32 page, Int32 pageSize
			)
			where TSource : class
		{
			if (pageSize>0)
				return source.Skip((Math.Max(page, 1) - 1) * pageSize).Take(pageSize);
			else
				return source;
		}

		/// <summary>
		/// представляет отсортированный запрос по массиву лямбда выражений
		/// </summary>
		/// <typeparam name="TSource">тип данных в источнике</typeparam>
		/// <param name="source">источник данных</param>
		/// <param name="keySelector">массив выражений для сортировки</param>
		/// <returns>отсортированный запрос</returns>
		public static IOrderedQueryable<TSource> OrderBy<TSource>(
				this IQueryable<TSource> source, Expression<Func<TSource, Object>>[] keySelector
			)
		{
			IOrderedQueryable<TSource> result=source.OrderBy(keySelector[0]);
			for (int i=1; i<keySelector.Length; i++)
				result=result.ThenBy(keySelector[i]);

			return result;
		}

		/// <summary>
		/// Выполняет фильтрацию последовательности значений на основе заданного правила-предиката.
		/// </summary>
		/// <typeparam name="TSource">The type of the source.</typeparam>
		/// <param name="source">IQueryable для фильтрации.</param>
		/// <param name="predicate">Правило для проверки каждого элемента на соответствие условию.</param>
		/// <returns>IQueryable, содержащий элементы из входной последовательности, которые удовлетворяют условию, указанному в predicate</returns>
		public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, Rule<TSource> predicate)
			where TSource : class 
			=> Queryable.Where(source, predicate.Expression);

		//public static Boolean Any<TSource>(this IEnumerable<TSource> source, Rule<TSource> predicate)
		//	where TSource : class
		//{
		//	return Queryable.Any(source.AsQueryable(), predicate.Expression);
		//}
	}
}
