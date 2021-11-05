namespace Kovi.Data.Cqrs
{
	/// <summary>
	/// Представляет обработчик запроса
	/// </summary>
	/// <typeparam name="TIn">Тип критерия запроса</typeparam>
	/// <typeparam name="TOut">Тип возвращаемых данных</typeparam>
	public interface IQueryHandler<in TIn, TOut>
		: IHandler<TIn, TOut>
		where TIn : IQriteria
	{ }

	/// <summary>
	/// Представляет асинхронный обработчик запроса
	/// </summary>
	/// <typeparam name="TIn">Тип критерия запроса</typeparam>
	/// <typeparam name="TOut">Тип возвращаемых данных</typeparam>
	public interface IAsyncQueryHandler<in TIn, TOut>
		: IAsyncHandler<TIn, TOut>
		where TIn : IQriteria
	{ }
}