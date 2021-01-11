using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Dapper;

using Kovi.Data.Cqrs;

namespace Kovi.Data.Dapper
{
	/// <summary>
	///     Dapper + QueryObject extensions
	/// </summary>
	public static class DapperQueryObjectExtensions
	{
		/// <summary>
		///     Dapper + QueryObject extension for Query
		/// </summary>
		public static IEnumerable<T> Query<T>(
			this QueryObject queryObject,
			IDbConnection conn,
			IDbTransaction transaction = null,
			bool buffered = true,
			int? commandTimeout = null,
			CommandType?
			commandType = null
		)
			=> conn.Query<T>(queryObject.Sql, queryObject.QueryParams, transaction, buffered, commandTimeout, commandType);

		public static T QueryFirstOrDefault<T>(
			this QueryObject queryObject,
			IDbConnection cnn,
			IDbTransaction transaction = null,
			int? commandTimeout = null,
			CommandType? commandType = null
		)
			=> cnn.QueryFirstOrDefault<T>(queryObject.Sql, queryObject.QueryParams, transaction, commandTimeout, commandType);

		public static Task<IEnumerable<T>> QueryAsync<T>(
			this QueryObject query,
			IDbConnection cnn,
			IDbTransaction transaction = null,
			int? commandTimeout = null,
			CommandType? commandType = null
		)
			=> cnn.QueryAsync<T>(query.Sql, query.QueryParams, transaction, commandTimeout, commandType);

		public static Task<T> QueryFirstOrDefaultAsync<T>(
			this QueryObject query,
			IDbConnection cnn,
			IDbTransaction transaction = null,
			int? commandTimeout = null,
			CommandType? commandType = null
		)
			=> cnn.QueryFirstOrDefaultAsync<T>(query.Sql, query.QueryParams, transaction, commandTimeout, commandType);

		public static int Execute(
			this QueryObject queryObject,
			IDbConnection conn,
			IDbTransaction transaction = null,
			int? commandTimeout = null,
			CommandType? commandType = null
		)
			=> conn.Execute(queryObject.Sql, queryObject.QueryParams, transaction, commandTimeout, commandType);
	}
}
