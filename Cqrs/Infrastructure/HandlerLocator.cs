using System;
using System.Threading.Tasks;
using Kovi.Data.Cqrs.Infrastructure;

namespace Kovi.Data.Cqrs
{
	/// <summary>
	/// Сервис, обеспечивающий поиск и выполнение команд и запросов.
	/// </summary>
	/// <inheritdoc cref="IHandlerService"/>
	public class HandlerLocator : IHandlerService
	{
		/// <summary>
		/// Объект-локатор, используемый для инжекции
		/// </summary>
		private Func<Type, Object> InstanceLocator { get; }

		#region .ctor

		public HandlerLocator(Func<Type, Object> instanceLocator)
		{
			InstanceLocator = instanceLocator ?? throw new ArgumentNullException(nameof(instanceLocator));
		}

		public HandlerLocator(InstanceFactory instanceFactory)
		{
			if (instanceFactory is null)
			{
				throw new ArgumentNullException(nameof(instanceFactory));
			}
			InstanceLocator = (TService => instanceFactory(TService));
		}

		public HandlerLocator(IInstanceFactory instanceFactory)
		{
			InstanceLocator = (TService => instanceFactory.Resolve(TService));
		}

		#endregion .ctor

		/// <summary>
		/// Возвращает типизированный объект из объект-локатора
		/// </summary>
		/// <typeparam name="TService">Запрашиваемый тип</typeparam>
		/// <returns>Объект</returns>
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
		Result ICommandHandlerService.Process<TCommand>(TCommand command)
		{
			throw new NotImplementedException();
			//// получим команду
			//var cmd = Get<ICommand<TCommand>>();

			//// запрос - проверка
			//var validateQuery = cmd.GetType()
			//	.GetCustomAttribute<ValidateCommandAttribute>();

			//Result result;
			//if (validateQuery != null)
			//	if (validateQuery.IsValid)
			//		result = ValidateQuery<TCommand>(InstanceFactory(validateQuery.ValidateQueryType), command, context);
			//	else
			//		throw new InvalidOperationException($"Validation query for command [{typeof(TCommand).Name}] is [{validateQuery.ValidateQueryType.Name}] that doesn't implement [{ValidateCommandAttribute.QueryType.Name}]");
			//else
			//	result = Result.Ok();

			//return result.OnSuccess(() =>
			//		Get<ICommand<TCommand>>()
			//		.Execute(command, context)
			//	);
		}

		Task<Result> ICommandHandlerService.ProcessAsync<TCommand>(TCommand command)
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

		//-----------------------------------------------------------------
		#region QueryHandlerLocator

		/// <inheritdoc/>
		IQueryHandlerService<TResponse> IQueryHandlerSpecificator.For<TResponse>()
			=> new QueryHandlerLocator<TResponse>(InstanceLocator);

		/// <summary>
		/// Вспомогательный тип для сервиса локатора запросов
		/// </summary>
		/// <typeparam name="TResponse">Тип запроса</typeparam>
		/// <inheritdoc cref="IQueryHandlerService{T}"/>
		private class QueryHandlerLocator<TResponse>
			: IQueryHandlerService<TResponse>
		{
			private Func<Type, Object> InstanceLocator { get; }

			#region ctor

			public QueryHandlerLocator(Func<Type, Object> instanceLocator)
			{
				InstanceLocator = instanceLocator ?? throw new ArgumentNullException(nameof(instanceLocator));
			}

			#endregion ctor

			protected TService Get<TService>()
				=> (TService)InstanceLocator(typeof(TService));

			/// <inheritdoc/>
			public IQueryHandler<TParam, TResponse> QueryHandler<TParam>(TParam param)
				where TParam : IQriteria
			{
				return Get<IQueryHandler<TParam, TResponse>>();
			}

			/// <inheritdoc/>
			public IAsyncQueryHandler<TParam, TResponse> AsyncQueryHandler<TParam>(TParam param)
				where TParam : IQriteria
			{
				return Get<IAsyncQueryHandler<TParam, TResponse>>();
			}

			/// <inheritdoc/>
			TResponse IQueryHandlerService<TResponse>.Ask<TParam>(TParam param)
			{
				return QueryHandler(param).Handle(param);
			}

			/// <inheritdoc/>
			async Task<TResponse> IQueryHandlerService<TResponse>.AskAsync<TParam>(TParam param)
			{
				return await AsyncQueryHandler(param).HandleAsync(param);
			}

			/// <summary>
			/// Выполняет поиск по Id
			/// </summary>
			/// <typeparam name="TParam">Тип-ответ</typeparam>
			/// <param name="param"></param>
			/// <returns>Результат поиска по Id.</returns>
			TResponse IQueryHandlerService<TResponse>.Find<TParam>(TParam param)
			{
				throw new NotImplementedException();
				//Get<IByIdQuery<TResponse>>()
				//.Ask(param, context);
			}
		}

		#endregion QueryHandlerLocator
	}
}