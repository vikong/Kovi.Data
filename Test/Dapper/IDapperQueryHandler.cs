using Kovi.Data.Cqrs;

namespace Data.Cqrs.Test.Dapper
{
	public interface IDapperQueryHandler<in TIn, TOut>
		: IQueryHandler<TIn, TOut>
		where TIn : IQriteria
	{ }
}