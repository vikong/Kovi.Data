namespace Kovi.Data.Dapper
{
	using Kovi.Data.Cqrs;

	/// <summary>
	/// Представляет запрос, который использует ORM Dapper для получения данных
	/// </summary>
	/// <typeparam name="TIn">Тип параметров запроса</typeparam>
	/// <typeparam name="TOut">Тип возвращаемых данных</typeparam>
	/// <inheritdoc cref="IQueryHandler{TIn, TOut}"/>
	public interface IDapperQuery<TIn, TOut>
		: IQueryHandler<TIn, TOut>, IQueryObject
		where TIn : IQriteria
	{ }
}