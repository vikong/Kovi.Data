using System;
using System.Reflection;
using System.Threading.Tasks;
using Kovi.Data.Cqrs.Infrastructure;

namespace Kovi.Data.Cqrs
{

	/// <summary>
	/// Сервис, обеспечивающий поиск и выполнение команд и запросов.
	/// </summary>
	[Obsolete]
	public class CqServiceLocator : ICqService
	{
		private Func<Type,Object> InstanceLocator { get; }

		private InstanceFactory InstanceFactory { get; }


		#region .ctor

		public CqServiceLocator(Func<Type, Object> instanceLocator)
		{
			InstanceLocator = instanceLocator ?? throw new ArgumentNullException(nameof(instanceLocator));
		}

		public CqServiceLocator(InstanceFactory instanceFactory)
		{
			InstanceFactory = instanceFactory ?? throw new ArgumentNullException(nameof(instanceFactory));
			InstanceLocator = (TService => instanceFactory(TService));
		}
		#endregion .ctor

		protected TService Get<TService>() 
			=> (TService)InstanceLocator(typeof(TService));

		#region CommandService

		//-----------------------------------------------------------------

		/// <summary>
		/// Выполняет указанную команду.
		/// </summary>
		/// <typeparam name="TCommand">Тип-команда</typeparam>
		/// <param name="command">Команда</param>
		/// <param name="context">Контекст</param>
		/// <returns>Результат выполнения команды</returns>
		Result ICommandService.Process<TCommand>(TCommand command, String context)
		{
			// получим команду
			var cmd = Get<ICommand<TCommand>>();

			// запрос - проверка
			var validateQuery = cmd.GetType()
				.GetCustomAttribute<ValidateCommandAttribute>();

			Result result;
			if (validateQuery != null)
				if (validateQuery.IsValid)
					result = ValidateQuery<TCommand>(InstanceFactory(validateQuery.ValidateQueryType), command, context);
				else
					throw new InvalidOperationException($"Validation query for command [{typeof(TCommand).Name}] is [{validateQuery.ValidateQueryType.Name}] that doesn't implement [{ValidateCommandAttribute.QueryType.Name}]");
			else
				result = Result.Ok();

			return result.OnSuccess(() =>
					Get<ICommand<TCommand>>()
					.Execute(command, context)
				);
		}

		Task<Result> ICommandService.ProcessAsync<TCommand>(TCommand command, String context)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Предварительный запрос - проверка
		/// </summary>
		/// <typeparam name="TParam">Тип-запрос</typeparam>
		/// <param name="query">Запрос типа IQuery{IQriteria,Result}</param>
		/// <param name="param">Параметры запроса</param>
		/// <param name="context">Контекст</param>
		/// <returns>Результат выполнения запроса.</returns>
		private Result ValidateQuery<TParam>(Object query, TParam param, String context)
			where TParam : IQriteria
			=> query == null ?
				Result.Ok()
				: ((IQuery<TParam, Result>)query).Ask(param, context);

		//-----------------------------------------------------------------

		#endregion CommandService

		#region QueryLocator

		//-----------------------------------------------------------------

		/// <summary>
		/// Определяет тип ответа.
		/// </summary>
		/// <typeparam name="TResponse">Тип-ответ</typeparam>
		/// <returns>Сервис выполнения запросов</returns>
		IQueryService<TResponse> IQueryLocator.For<TResponse>()
			=> new QueryLocator<TResponse>(InstanceLocator);

		/// <summary>
		/// Вспомогательный тип для сервиса запросов
		/// </summary>
		/// <typeparam name="TResponse">Тип запроса</typeparam>
		private class QueryLocator<TResponse>
			: IQueryService<TResponse>
		{
			private Func<Type, Object> InstanceLocator { get; }
			//private readonly IServiceLocator _serviceLocator;
			//private InstanceFactory InstanceFactory;

			#region ctor
			public QueryLocator(Func<Type, Object> instanceLocator)
			{
				InstanceLocator = instanceLocator ?? throw new ArgumentNullException(nameof(instanceLocator));
			}

			//public QueryLocator(IServiceLocator serviceLocator)
			//	=> _serviceLocator = serviceLocator;

			#endregion ctor

			protected TService Get<TService>()
				=> (TService)InstanceLocator(typeof(TService));

			public IQuery<TParam, TResponse> Query<TParam>(TParam param)
				where TParam:IQriteria
				=> Get<IQuery<TParam, TResponse>>();

			/// <summary>
			/// Выполняет запрос
			/// </summary>
			/// <typeparam name="TParam">Тип-запрос</typeparam>
			/// <param name="param">Параметры запроса</param>
			/// <returns>Результат выполнения запроса</returns>
			TResponse IQueryService<TResponse>.Ask<TParam>(TParam param, String context)
				=> Query(param).Ask(param, context);

			/// <summary>
			/// Выполняет асинхронный запрос
			/// </summary>
			/// <typeparam name="TParam">Тип-запрос</typeparam>
			/// <param name="param">Параметры запроса</param>
			/// <returns>Результат выполнения запроса</returns>
			Task<TResponse> IQueryService<TResponse>.AskAsync<TParam>(TParam param, String context)
				=> Query(param).AskAsync(param, context);

			/// <summary>
			/// Выполняет поиск по Id
			/// </summary>
			/// <typeparam name="TParam">Тип-ответ</typeparam>
			/// <param name="param"></param>
			/// <returns>Результат поиска по Id.</returns>
			TResponse IQueryService<TResponse>.Find<TParam>(TParam param, String context)
				=> Get<IByIdQuery<TResponse>>()
				.Ask(param, context);
		}

		#endregion QueryLocator
	}
}

//private Result Validate<TCommand>(TCommand command, String connectionString = null)
//	where TCommand : ICmdParam
//{
//	// Проверяем данные
//	var chkQuery = GetQueriedValidator<TCommand,Result>(command);
//	if (chkQuery == null)
//		return Result.Ok();

//	return chkQuery.Ask(command, connectionString);
//}

//private IQuery<TParam, TResponse> GetQueriedValidator<TParam, TResponse>(TParam param)
//	where TParam : IQriteria
//	=> _serviceLocator.TryGetInstance<IQuery<TParam, TResponse>>();