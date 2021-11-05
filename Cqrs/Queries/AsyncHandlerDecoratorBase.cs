using System.Threading.Tasks;

namespace Kovi.Data.Cqrs
{
	public abstract class AsyncHandlerDecoratorBase<TIn, TOut>
		: IAsyncHandler<TIn, TOut>
	{
		protected readonly IHandler<TIn, TOut> Decorated;

		public AsyncHandlerDecoratorBase(IHandler<TIn, TOut> decorated)
		{
			Decorated = decorated;
		}

		public abstract Task<TOut> HandleAsync(TIn input);
	}

}