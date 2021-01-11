using System;
using System.Data;
using Kovi.Data.Cqrs;

namespace Data.Cqrs.Test.Dapper
{
	public class DbConnectionFactory : IConnectionFactory
	{
		public IDbConnection Create(String connectionString = null)
		{
			return new System.Data.SqlClient.SqlConnection(@"Data Source=NTB00382;Initial Catalog=BookStore;Integrated Security=True");
		}
	}
}
