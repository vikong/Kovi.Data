using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Kovi.LinqExtensions.Expressions
{
	public static class ExpressionHelper
	{
		private static readonly ConcurrentDictionary<Expression, Lazy<Object>> _cache
			= new ConcurrentDictionary<Expression, Lazy<Object>>();

		/// <summary>
		/// Конвертирует Expression в функцию. Использует кэш для скомпилированных выражений.
		/// </summary>
		/// <typeparam name="TIn">The type of the in.</typeparam>
		/// <typeparam name="TOut">The type of the out.</typeparam>
		/// <param name="expr">Expression.</param>
		/// <returns>Скомпилированное выражение</returns>
		public static Func<TIn, TOut> AsFunc<TIn, TOut>(this Expression<Func<TIn, TOut>> expr)
		{
			// Используется Lazy<object>, чтобы предотвратить возможную лишнюю компиляцию, 
			// при одновременном вызове из разных потоков.
			return (Func<TIn, TOut>)_cache.GetOrAdd
				(
					expr,
					id => new Lazy<Object>(() => expr.Compile())
				).Value;
		}
	}

}
