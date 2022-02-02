using System;

namespace Kovi.Data.Cqrs.Linq
{
	/// <summary>
	/// Обработчик запросов Linq
	/// </summary>
	/// <typeparam name="TIn">Тип-Параметр</typeparam>
	/// <typeparam name="TOut">Тип-Результат</typeparam>
	/// <inheritdoc cref="IQueryHandler{TIn, TOut}"/>
	public class LinqQueryHandler<TIn, TOut> 
		: IQueryHandler<TIn, TOut>
		where TIn : IQriteria
	{
		protected readonly ILinqProviderFactory LinqProviderFactory;

		protected readonly ILinqQuery<TIn, TOut> Query;

		public LinqQueryHandler(ILinqQuery<TIn, TOut> query, ILinqProviderFactory linqProviderFactory)
		{
			LinqProviderFactory = linqProviderFactory;
			Query = query;
		}

		public TOut Handle(TIn qrit)
		{
			string connection = qrit is IConnection ?
				((IConnection)qrit).Connection :
				null;

			using (ILinqProvider linqProvider = LinqProviderFactory.Create(connection))
			{
				return Query.Handle(qrit, linqProvider);
			}
		}
	}

}
