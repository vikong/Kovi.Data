using System;
using System.Data;
using Kovi.Data.Cqrs;

namespace Data.Cqrs.Test.Dapper
{
	public class BooksConnectionFactory : IConnectionFactory
	{
		private string DefaultConnection 
			=> @"Data Source=NTB00382;Initial Catalog=BookStore;Integrated Security=True";
		
		public IDbConnection Create(String connectionString = null)
		{
			string connStr = connectionString ?? DefaultConnection;
			return new System.Data.SqlClient.SqlConnection(connStr);
		}
	}
}
