namespace Kovi.Data.Cqrs
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
		public QueryObject(String sql)
		{
			if (String.IsNullOrEmpty(sql))
				throw new ArgumentNullException("Empty SQL.");

			Sql = sql;
		}

		/// <summary>
		///     Create QueryObject for parameterized <paramref name="sql" />
		/// </summary>
		/// <param name="sql">SQL string</param>
		/// <param name="queryParams">Parameter list</param>
		public QueryObject(String sql, Object queryParams) : this(sql)
		{
			QueryParams = queryParams;
		}

		#endregion .ctor

		/// <summary>
		///     SQL string
		/// </summary>
		public String Sql { get; private set; }

		/// <summary>
		///     Parameter list
		/// </summary>
		public Object QueryParams { get; private set; }

		public override String ToString() => Sql;
	}
}