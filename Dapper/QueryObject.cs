namespace Kovi.Data.Dapper
{
	using System;

	/// <summary>
	///     Incapsulate SQL and Parameters for Dapper methods
	/// </summary>
	/// <remarks>
	///     http://www.martinfowler.com/eaaCatalog/queryObject.html
	/// </remarks>
	public class QueryObject
	{
		#region .ctor

		/// <summary>
		///     Create QueryObject for <paramref name="sql" /> string only
		/// </summary>
		/// <param name="sql">SQL string</param>
		public QueryObject(string sql)
		{
			if (string.IsNullOrEmpty(sql))
				throw new ArgumentNullException("Empty SQL.");

			Sql = sql;
		}

		/// <summary>
		///     Create QueryObject for parameterized <paramref name="sql" />
		/// </summary>
		/// <param name="sql">SQL string</param>
		/// <param name="queryParams">Parameter list</param>
		public QueryObject(string sql, object queryParams)
			: this(sql)
		{
			QueryParams = queryParams;
		}

		#endregion .ctor

		/// <summary>
		///     SQL string
		/// </summary>
		public string Sql { get; private set; }

		/// <summary>
		///     Parameter list
		/// </summary>
		public object QueryParams { get; private set; }

		public override string ToString() => Sql;
	}
}