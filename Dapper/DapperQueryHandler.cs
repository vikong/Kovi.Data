using System;
using System.Data;
using Kovi.Data.Cqrs;
using Dapper;

namespace Kovi.Data.Dapper
{
	/// <summary>
	/// Представляет обработчик запросов Dapper
	/// </summary>
	/// <typeparam name="TIn">Тип параметров запроса</typeparam>
	/// <typeparam name="TOut">Тип возвращаемых данных</typeparam>
	public interface IDapperQueryHandler<TIn, TOut>
	{
		/// <summary>
		/// Выполняет запрос Dapper
		/// </summary>
		/// <typeparam name="T">Тип данных, возвращаемых Dapper</typeparam>
		/// <param name="query">Запрос</param>
		/// <returns>Результат выполнения</returns>
		TOut Handle<T>(IDapperQuery<TIn, T, TOut> query);

		/// <summary>
		/// Выполняет запрос Dapper с конвертацией
		/// </summary>
		/// <typeparam name="T">Тип данных, возвращаемых Dapper после конвертации</typeparam>
		/// <param name="query">Запрос</param>
		/// <returns>Результат выполнения</returns>
		TOut Handle<T>(IDapperMapQuery<TIn, T, TOut> query);
	}

	/// <summary>
	/// Обработчик запросов Dapper
	/// </summary>
	/// <typeparam name="TIn">Тип параметров запроса</typeparam>
	/// <typeparam name="TOut">Тип возвращаемых данных</typeparam>
	/// <inheritdoc cref="IDapperQueryHandler{TIn, TOut}"/>
	public class DapperQueryHandler<TIn, TOut>
		: IDapperQueryHandler<TIn, TOut>
		where TIn : IQriteria
	{
		/// <summary>
		/// Генератор <see cref="IDbConnection"/>
		/// </summary>
		protected readonly IConnectionFactory ConnectionFactory;

		public DapperQueryHandler(IConnectionFactory connectionFactory)
		{
			ConnectionFactory = connectionFactory ?? throw new NullReferenceException(nameof(connectionFactory));
		}

		protected IDbConnection CreateConnection(TIn param)
		{
			string connString = param is IConnection connection ?
				connection.Connection :
				null;

			return ConnectionFactory.Create(connString);
		}

		public TOut Handle<T>(IDapperQuery<TIn, T, TOut> query)
		{
			using (IDbConnection conn = CreateConnection(query.Param))
			{
				var buff = conn.Query<T>(query.Sql, query.Param);
				return query.Convert(buff);
			}
		}

		public TOut Handle<T>(IDapperMapQuery<TIn, T, TOut> query)
		{
			using (IDbConnection conn = CreateConnection(query.Param))
			{
				var buff = conn.Query(query.Sql,
					query.SplitTypes,
					map: query.MapFunc,
					splitOn: query.SplitOn,
					param: query.Param);

				return query.Convert(buff);
			}
		}

	}

}