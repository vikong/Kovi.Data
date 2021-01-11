using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kovi.LinqExtensions.Expressions
{
	public static class PredicateHelper
	{
		public static Expression<Func<T, Boolean>> Create<T>(Expression<Func<T, Boolean>> predicate)
			=> predicate;

		public static Expression< Func<TIn,Boolean> > Create<TIn,TPredicate>(Expression<Func<TPredicate, Boolean>> predicateExpression, Expression< Func<TIn, TPredicate> > convertExpression)
			=> convertExpression.Predicated(predicateExpression);

		public static Expression<Func<T, Boolean>> AndIf<T>(this Expression<Func<T, Boolean>> inExpression, Boolean condition, Expression<Func<T, Boolean>> andExpression)
			=> condition ? inExpression.And(andExpression) : inExpression;

		public static Expression<Func<T1, Boolean>> Predicated<T1, T2>(this Expression<Func<T1, T2>> converterExpr, Expression<Func<T2, Boolean>> predicate)
			=> Expression.Lambda<Func<T1, Boolean>>(predicate.Apply(converterExpr.Body), converterExpr.Parameters.First());

		public static Expression<Func<T1, Boolean>> With<T1, T2>(this Expression<Func<T2, Boolean>> predicate, Expression<Func<T1, T2>> converterExpr)
			=> Expression.Lambda<Func<T1, Boolean>>(predicate.Apply(converterExpr.Body), converterExpr.Parameters.First());

		public static Boolean Is<T>(this T entity, Expression<Func<T, Boolean>> expr) 
			where T : class 
			=> expr.AsFunc().Invoke(entity);

		public static Boolean IsSatisfiedBy<T>(this Expression<Func<T, Boolean>> expr, T item)
			=> (new T[] { item }.AsQueryable()).Where(expr).Any();


		/// <summary>
		/// Строит выражение вида e => e IN (list of TElements).
		/// </summary>
		/// <typeparam name="TElement">Тип элемента.</typeparam>
		/// <typeparam name="TValue">Тип значений.</typeparam>
		/// <param name="valueSelector">The value selector.</param>
		/// <param name="values">The values.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">
		/// valueSelector
		/// or
		/// values
		/// </exception>
		public static Expression<Func<TElement, Boolean>> In<TElement, TValue>
			(
				this Expression<Func<TElement, TValue>> valueSelector,
				IEnumerable<TValue> values
			)
		{
			if (null == valueSelector)
				throw new ArgumentNullException(nameof(valueSelector));

			if (null == values)
				throw new ArgumentNullException(nameof(values));

			ParameterExpression p = valueSelector.Parameters.Single();

			if (!values.Any())
				return e => false;

			var equals = values.Select(value =>
				(Expression)Expression.Equal(
					 valueSelector.Body,
					 Expression.Constant(
						 value,
						 typeof(TValue)
					 )
				)
			);

			var body = equals.Aggregate<Expression>(
					 (accumulate, equal) => Expression.OrElse(accumulate, equal)
			 );

			return Expression.Lambda<Func<TElement, Boolean>>(body, p);
		}

	}

}
