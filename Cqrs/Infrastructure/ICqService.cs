namespace Kovi.Data.Cqrs
{
	using System;
	using System.Threading.Tasks;

	public interface ICqService : IQueryLocator, ICommandService
	{ }

	public interface IQueryLocator
	{
		/// <summary>
		/// Определяет тип результата выполнения запроса
		/// </summary>
		/// <typeparam name="TResponse">Тип-результат</typeparam>
		/// <returns>Сервис выполнения запросов</returns>
		IQueryService<TResponse> For<TResponse>();
	}

	public interface IQueryService<TResponse>
	{
		IQuery<TParam, TResponse> Query<TParam>(TParam param)
			where TParam : IQriteria;

		TResponse Ask<TParam>(TParam param, String context = null)
			where TParam : IQriteria;

		Task<TResponse> AskAsync<TParam>(TParam param, String context = null)
			where TParam : IQriteria;

		TResponse Find<TParam>(TParam param, String context = null)
			where TParam : IQriteria, IHasId;
	}

	public interface ICommandService
	{
		Result Process<TCommand>(TCommand command, String context = null)
			where TCommand : ICmdParam;

		Task<Result> ProcessAsync<TCommand>(TCommand command, String context = null)
			where TCommand : ICmdParam;
	}
}