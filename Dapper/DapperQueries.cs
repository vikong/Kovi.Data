using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Kovi.Data.Cqrs;

namespace Kovi.Data.Dapper
{
	/// <summary>
	/// Базовый класс для запросов с использованием Dapper.
	/// </summary>
	/// <typeparam name="TParam">Критерий запроса.</typeparam>
	/// <typeparam name="TResponse">Результат.</typeparam>
	public abstract class DapperQuery<TParam, TResponse>
		: IQuery<TParam, TResponse>
		where TParam : IQriteria
	{
		private readonly IQueryObjectHandler<TResponse> QueryHandler;

		public abstract QueryObject Query(TParam param);

		public TResponse Ask(TParam param, string source = null)
			=> QueryHandler.Ask(Query(param), source);

		public async Task<TResponse> AskAsync(TParam param, string source = null)
			=> await QueryHandler.AskAsync(Query(param), source);

		#region .ctor

		protected DapperQuery(IQueryObjectHandler<TResponse> handler)
		{
			QueryHandler = handler;
		}

		#endregion .ctor

	}

	/// <summary>
	/// Базовый класс для запросов с использованием Dapper, возвращающих одну строку.
	/// </summary>
	/// <typeparam name="TParam">Критерий запроса.</typeparam>
	/// <typeparam name="TResponse">Результат.</typeparam>
	public abstract class SingleDapperQuery<TParam, TResponse>
		: DapperQuery<TParam, TResponse>
		where TParam : IQriteria
	{
		protected SingleDapperQuery(IConnectionFactory connectionFactory)
			: base(new SingleDapperHandlder<TResponse>(connectionFactory))
		{ }
	}

	/// <summary>
	/// Базовый класс для запросов с использованием Dapper, возвращающих несколько строк.
	/// </summary>
	/// <typeparam name="TParam">Критерий запроса.</typeparam>
	/// <typeparam name="TResponse">Результат.</typeparam>
	public abstract class EnumDapperQuery<TParam, TResponse>
		: DapperQuery<TParam, IEnumerable<TResponse>>
		where TParam : IQriteria
	{
		protected EnumDapperQuery(IConnectionFactory connectionFactory)
			: base(new EnumDapperHandlder<TResponse>(connectionFactory))
		{ }
	}


}
