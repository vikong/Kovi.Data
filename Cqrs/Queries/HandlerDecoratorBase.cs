namespace Kovi.Data.Cqrs
{
	public abstract class HandlerDecoratorBase<TIn, TOut>
		: IHandler<TIn, TOut>
	{
		protected readonly IHandler<TIn, TOut> Decorated;

		public HandlerDecoratorBase(IHandler<TIn, TOut> decorated)
		{
			Decorated = decorated;
		}

		public abstract TOut Handle(TIn input);
	}

}