namespace Kovi.Data.Cqrs.Linq
{
	public interface ILinqQuery<TIn, TOut>
		where TIn : IQriteria
	{
		TOut Handle(TIn qrit, ILinqProvider linq);
	}
}