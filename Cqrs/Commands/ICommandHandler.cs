namespace Kovi.Data.Cqrs
{
	public interface ICommandHandler<in TIn>
		: IHandler<TIn, Result>
		where TIn : ICommand
	{ }

	public interface IAsyncCommandHandler<in TIn>
		: IAsyncHandler<TIn, Result>
		where TIn : ICommand
	{ }
}