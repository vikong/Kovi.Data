using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Kovi.Data.Cqrs;

namespace Kovi.Data.Dapper
{
	public interface IEnumDapperHandler<TResponse>
	: IQueryObjectHandler<IEnumerable<TResponse>>
	{ }

	public interface ISingleDapperHandler<TResponse>
		: IQueryObjectHandler<TResponse>
	{ }

	/// <summary>
	/// Базовый класс обработчика для запросов с использованием Dapper, возвращающих несколько строк.
	/// </summary>
	/// <typeparam name="TResponse">Результат.</typeparam>
	public class EnumDapperHandlder<TResponse>
		: BaseQueryObjectHandler<IEnumerable<TResponse>>
		, IEnumDapperHandler<TResponse>
	{
		public EnumDapperHandlder(IConnectionFactory connectionFactory)
			: base(connectionFactory)
		{ }

		protected override IEnumerable<TResponse> Ask(IDbConnection connection, QueryObject query)
		 => query.Query<TResponse>(connection);

		protected override Task<IEnumerable<TResponse>> AskAsync(IDbConnection connection, QueryObject query)
			=> query.QueryAsync<TResponse>(connection);
	}

	/// <summary>
	/// Базовый класс обработчика для запросов с использованием Dapper, возвращающих одну строку.
	/// </summary>
	/// <typeparam name="TResponse">Результат.</typeparam>
	public class SingleDapperHandlder<TResponse>
		: BaseQueryObjectHandler<TResponse>
		, ISingleDapperHandler<TResponse>
	{
		public SingleDapperHandlder(IConnectionFactory connectionFactory)
			: base(connectionFactory)
		{ }

		protected override TResponse Ask(IDbConnection connection, QueryObject query)
		 => query.QueryFirstOrDefault<TResponse>(connection);

		protected override Task<TResponse> AskAsync(IDbConnection connection, QueryObject query)
			=> query.QueryFirstOrDefaultAsync<TResponse>(connection);
	}
}