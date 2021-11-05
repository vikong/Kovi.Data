namespace Kovi.Data.Cqrs.Linq
{
	public interface ILinqQueryOld<TIn, TOut>
		where TIn : IQriteria
	{
		TIn Qrit { get; }

		TOut Ask(ILinqProvider linqProvider);
	}

	public interface ILinqQuery<TIn, TOut>
		where TIn : IQriteria
	{
		TOut Handle(TIn qrit, ILinqProvider linq);
	}
}