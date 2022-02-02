namespace Kovi.Data.Dapper
{
	using System.Collections.Generic;

	/// <summary>
	/// Представляет обработчик запроса <see cref="IQueryObject"/>
	/// </summary>
	/// <typeparam name="TOut">Тип возвращаемых данных</typeparam>
	public interface IQueryObjectHandler<TOut>
	{
		/// <summary>
		/// Выполняет запрос
		/// </summary>
		/// <param name="query">Запрос</param>
		/// <returns>Полученные данные</returns>
		TOut Handle(IQueryObject query);
	}

	/// <summary>
	/// Представляет обработчик запроса <see cref="IQueryObject"/>, с возвратом единственной записи
	/// </summary>
	/// <typeparam name="TOut">Тип возвращаемых данных</typeparam>
	/// <inheritdoc cref="IQueryObjectHandler{TOut}"/>
	public interface ISingleQueryObjectHandler<TOut>
		: IQueryObjectHandler<TOut>
		where TOut: class
	{ }

	/// <summary>
	/// Представляет обработчик запроса <see cref="IQueryObject"/>, с возвратом нескольких записей
	/// </summary>
	/// <typeparam name="TOut">Тип возвращаемых данных</typeparam>
	/// <inheritdoc cref="IQueryObjectHandler{TOut}"/>
	public interface IEnumQueryObjectHandler<TOut>
	: IQueryObjectHandler<IEnumerable<TOut>>
	{ }

}