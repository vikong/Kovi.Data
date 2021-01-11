using System;
using System.Data;
using System.Threading.Tasks;

namespace Kovi.Data.Cqrs
{

	public abstract class BaseQueryObjectHandler<TResponse>
		: IQueryObjectHandler<TResponse>
	{
		private readonly IConnectionFactory _connectionFactory;

		abstract protected TResponse Ask(IDbConnection connection, QueryObject query);

		abstract protected Task<TResponse> AskAsync(IDbConnection connection, QueryObject query);

		protected BaseQueryObjectHandler(IConnectionFactory connectionFactory)
		{
			_connectionFactory = connectionFactory ?? throw new NullReferenceException(nameof(connectionFactory));
		}

		public TResponse Ask(QueryObject query, String source = null)
		{
			try
			{
				using (IDbConnection dbConn = _connectionFactory.Create(source))
				{
					return Ask(dbConn, query);
				}
			}
			catch (QueryException ex) { throw ex; }
			catch (Exception ex)
			{
				throw BuildException(query, ex);
			}
		}

		public async Task<TResponse> AskAsync(QueryObject query, String source = null)
		{
			try
			{
				using (IDbConnection dbConn = _connectionFactory.Create(source))
				{
					return await AskAsync(dbConn, query);
				}
			}
			catch (QueryException ex) { throw ex; }
			catch (Exception ex)
			{
				throw BuildException(query, ex);
			}
		}

		protected Exception BuildException(QueryObject query, Exception ex, IDbConnection dbConn = null, String message = null)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder("Error during handle query.\r\n");
			sb.AppendLine($"Query: {query}");
			if (dbConn != null)
				sb.AppendLine(dbConn.ToString());

			return new QueryException(sb.ToString(), ex);
		}
	}

}