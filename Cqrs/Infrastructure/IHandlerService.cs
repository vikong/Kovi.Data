namespace Kovi.Data.Cqrs
{
	using System.Threading.Tasks;

	/// <summary>
	/// Представляет сервис, обеспечивающий поиск и выполнение команд и запросов.
	/// </summary>
	public interface IHandlerService
		: IQueryHandlerSpecificator, ICommandHandlerService
	{ }

	/// <summary>
	/// Представляет вспомогательный сервис, уточняющий тип ответа запроса.
	/// </summary>
	public interface IQueryHandlerSpecificator
	{
		/// <summary>
		/// Определяет тип результата выполнения запроса
		/// </summary>
		/// <typeparam name="TResponse">Тип-результат</typeparam>
		/// <returns>Сервис выполнения запросов</returns>
		IQueryHandlerService<TResponse> For<TResponse>();
	}

	/// <summary>
	/// Представляет сервис, обеспечивающий поиск и выполнение запросов.
	/// </summary>
	/// <typeparam name="TResponse">Тип возвращаемых данных</typeparam>
	public interface IQueryHandlerService<TResponse>
	{
		/// <summary>
		/// Возвращает обработчик запроса, соответствующий параметру и запрашиваемому типу ответа
		/// </summary>
		/// <typeparam name="TParam">Тип критерия</typeparam>
		/// <param name="param">Критерий запроса</param>
		/// <returns>Обработчик запроса </returns>
		IQueryHandler<TParam, TResponse> QueryHandler<TParam>(TParam param)
			where TParam : IQriteria;

		/// <summary>
		/// Возвращает обработчик асинхронного запроса, соответствующий параметру и запрашиваемому типу ответа
		/// </summary>
		/// <typeparam name="TParam">Тип критерия</typeparam>
		/// <param name="param">Критерий запроса</param>
		/// <returns>Обработчик запроса</returns>
		IAsyncQueryHandler<TParam, TResponse> AsyncQueryHandler<TParam>(TParam param)
			where TParam : IQriteria;

		/// <summary>
		/// Выполняет запрос, соответствующий типу критерия и ответа
		/// </summary>
		/// <typeparam name="TParam">Тип критерия</typeparam>
		/// <param name="param">Критерий запроса</param>
		/// <returns>Результат выполнения запроса</returns>
		TResponse Ask<TParam>(TParam param)
			where TParam : IQriteria;

		/// <summary>
		/// Выполняет асинхронный запрос, соответствующий типу критерия и ответа
		/// </summary>
		/// <typeparam name="TParam">Тип критерия</typeparam>
		/// <param name="param">Критерий запроса</param>
		/// <returns>Результат выполнения запроса</returns>
		Task<TResponse> AskAsync<TParam>(TParam param)
			where TParam : IQriteria;

		TResponse Find<TParam>(TParam param)
			where TParam : IQriteria, IHasId;
	}

	/// <summary>
	/// Представляет сервис, обеспечивающий поиск и выполнение команд.
	/// </summary>
	public interface ICommandHandlerService
	{
		Result Process<TCommand>(TCommand command)
			where TCommand : ICommand;

		Task<Result> ProcessAsync<TCommand>(TCommand command)
			where TCommand : ICommand;
	}
}