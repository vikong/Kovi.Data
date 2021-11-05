using System;

namespace Kovi.Data.Cqrs.Linq
{

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

	//public class LinqQueryHandlerOld<TIn, TOut> : ILinqQueryHandlerOld<TIn, TOut>
	// where TIn : IQriteria
	//{
	//	protected readonly ILinqProviderFactory LinqProviderFactory;

	//	public LinqQueryHandlerOld(ILinqProviderFactory linqProviderFactory)
	//	{
	//		LinqProviderFactory = linqProviderFactory;
	//	}

	//	public TOut Handle(ILinqQueryOld<TIn, TOut> query)
	//	{
	//		string connection = query.Qrit is IConnection ?
	//			((IConnection)query.Qrit).Connection :
	//			null;

	//		using (ILinqProvider linqProvider = LinqProviderFactory.Create(connection))
	//		{
	//			return query.Ask(linqProvider);
	//		}
	//	}
	//}


}
