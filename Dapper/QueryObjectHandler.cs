using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Dapper;
using Kovi.Data.Cqrs;

namespace Kovi.Data.Dapper
{
	/// <summary>
	/// Базовый класс выполнения запросов с использованием Dapper
	/// </summary>
	/// <typeparam name="TOut">Тип возвращаемых данных</typeparam>
	/// <inheritdoc cref="IQueryObjectHandler{TOut}"/>
	public abstract class BaseQueryObjectHandler<TOut> 
		: IQueryObjectHandler<TOut>
	{

		private readonly IConnectionFactory connectionFactory;

		public BaseQueryObjectHandler(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory ?? throw new NullReferenceException(nameof(connectionFactory)); ;
		}

		/// <summary>
		/// Выполняет запрос с использованием <see cref="IDbConnection"/> 
		/// </summary>
		/// <param name="query">Запрос</param>
		/// <param name="connection">Подключение</param>
		/// <returns>Результат</returns>
		protected abstract TOut HandleQuery(IQueryObject query, IDbConnection connection);

		public TOut Handle(IQueryObject query)
		{
			try
			{
				using (IDbConnection conn = connectionFactory.Create())
				{
					return HandleQuery(query, conn);
				}
			}
			catch (QueryException ex) 
			{ 
				throw ex; 
			}
			catch (Exception ex)
			{
				throw BuildException(query, ex);
			}

		}

		protected Exception BuildException(IQueryObject query, Exception ex, IDbConnection dbConn = null, String message = null)
		{
			StringBuilder sb = new StringBuilder("Error during handle query.\r\n");
			sb.AppendLine($"Query: {query}");
			if (dbConn != null)
				sb.AppendLine(dbConn.ToString());

			return new QueryException(sb.ToString(), ex);
		}

	}

	/// <summary>
	/// Выполняет запрос с возвратом единственной записи
	/// </summary>
	/// <typeparam name="TOut">Тип возвращаемых данных</typeparam>
	/// <inheritdoc cref="ISingleQueryObjectHandler{TOut}"/>
	public class SingleQueryObjectHandler<TOut>
		: BaseQueryObjectHandler<TOut>,
		ISingleQueryObjectHandler<TOut>
		where TOut:class
	{
		public SingleQueryObjectHandler(IConnectionFactory connectionFactory)
			: base(connectionFactory)
		{ }

		protected override TOut HandleQuery(IQueryObject query, IDbConnection conn)
		{
			return conn.QuerySingleOrDefault<TOut>(query.Sql, query.QueryParams);
		}
	}

	/// <summary>
	/// Выполняет запрос с возвратом набора записей типа <see cref="TOut"/>
	/// </summary>
	/// <typeparam name="TOut">Тип возвращаемых данных</typeparam>
	/// <inheritdoc cref="IEnumQueryObjectHandler{TOut}"/>
	public class EnumQueryObjectHandler<TOut>
		: BaseQueryObjectHandler<IEnumerable<TOut>>,
		IEnumQueryObjectHandler<TOut>
	{
		public EnumQueryObjectHandler(IConnectionFactory connectionFactory)
			: base(connectionFactory)
		{ }

		protected override IEnumerable<TOut> HandleQuery(IQueryObject query, IDbConnection conn)
		{
			return conn.Query<TOut>(query.Sql, query.QueryParams);
		}
	}

}