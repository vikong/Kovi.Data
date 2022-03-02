using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Kovi.Data;

namespace Data.Cqrs.Test.Tests.Dapper
{
	[TestClass]
	public class QueryBuilderTest
	{
		[TestMethod]
		public void Expression_ChainAnd_GenerateAnd()
		{
			var expr = new WhereExpression("Id=@p").And("Name=@n");
			Assert.AreEqual("(Id=@p) AND (Name=@n)", expr.Expression, true);

		}
		[TestMethod]
		public void Expression_ChainOr_GenerateOr()
		{
			var expr = new WhereExpression("Id=@p").Or("Name=@n");
			Assert.AreEqual("(Id=@p) OR (Name=@n)", expr.Expression, true);

		}

		[TestMethod]
		public void MyTestMethod()
		{
			var query = SqlQueryBuilder.Select("*")
				.From("table1 t1 inner join table2 t2");

			Assert.AreEqual("SELECT * from table1 t1 inner join table2 t2", query.Sql, true);
		}

		[TestMethod]
		public void ChainWhere_Generate_AndWhere()
		{
			
			var query = SqlQueryBuilder.Select("*")
				.From("table1", "t1")
				.Where("t1.Name=@n")
				.Where("t1.Year=@y");

			string expected = "SELECT * FROM table1 t1 WHERE (t1.Name=@n) AND (t1.Year=@y)";

			Assert.AreEqual(expected, query.Sql, true);
		}
		[TestMethod]
		public void GroupBy_Generate_GroupBy()
		{
			var query = SqlQueryBuilder.Select("*")
				.From("table1 t1")
				.Where("t1.Name=@n")
				.GroupBy("t1.Year");

			string expected = "SELECT * FROM table1 t1 WHERE t1.Name=@n GROUP BY t1.Year";

			Assert.AreEqual(expected, query.Sql, true);
		}
	}
}
