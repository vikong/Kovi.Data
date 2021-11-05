using System;
using Kovi.Data.Cqrs;
using Kovi.Data.Cqrs.Linq;

namespace Data.Cqrs.Test
{
	public class LinqQueryHandlerStub<TIn, TOut> : IQueryHandler<TIn, TOut>
        where TIn : IQriteria
    {
        protected readonly ILinqProviderFactory LinqProviderFactory;
        protected readonly ILinqQuery<TIn, TOut> Query;

        public LinqQueryHandlerStub(ILinqQuery<TIn, TOut> query)
        {
            Query = query;
        }

        public TOut Handle(TIn qrit)
        {
            var result = Query.Handle(qrit, null);

            var qr = result as QueryResult<String>;
            if (qr != null)
            {
                qr.From("LinqQueryHandlerStub");
            }
            return result;
        }
    }

}
