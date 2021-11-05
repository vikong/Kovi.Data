using System.Diagnostics;
using Kovi.Data.Cqrs;

namespace Data.Cqrs.Test
{
	public class TestDecoratorQueryHandler<TIn, TOut>
		: HandlerDecoratorBase<TIn, TOut>
		, IQueryHandler<TIn, TOut>
		where TIn : IQriteria
	{

		public TestDecoratorQueryHandler(IQueryHandler<TIn, TOut> decorated)
			: base(decorated)
		{ }

		public override TOut Handle(TIn input)
		{
			var result = Decorated.Handle(input);

			var qr = result as QueryResult<string>;
			if (qr != null)
			{
				qr.From("TestDecoratorQueryHandler");
			}
			return result;
		}
	}
}
