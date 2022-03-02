using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kovi.Data.Dapper;

namespace Kovi.Data
{
	public class SqlQueryBuilder : ISql
	{
		public static class Clause
		{
			public static String Select = "SELECT";
			public static String From = "FROM";
			public static String Where = "WHERE";
			public static String GroupBy = "GROUP BY";
			public static String OrderBy = "ORDER BY";
		}
		public String SelectExpession { get; set; } = "*";
		public String FromExpression { get; set; }
		public IBooleanExpression WhereExpression { get; set; }
		public String GroupExpression { get; set; }
		public String OrderExpression { get; set; }

		private String FormatClause(String clause, String expression)
		{
			if (!String.IsNullOrWhiteSpace(expression))
			{ 
				return $"{clause} {expression.Trim()} ";
			}
			return String.Empty;
		}

		public String Sql
		{
			get
			{
				StringBuilder sb = new StringBuilder($"{Clause.Select} ");
				sb.Append(SelectExpession).Append(" ");
				
				//From
				sb.Append(FormatClause(Clause.From, FromExpression));

				//Where
				if (WhereExpression != null)
				{
					sb.Append(FormatClause(Clause.Where, WhereExpression.Expression));
				}

				//Group By
				if (!String.IsNullOrWhiteSpace(GroupExpression))
				{
					sb.Append(FormatClause(Clause.GroupBy, GroupExpression));
				}

				//Order By
				if (!String.IsNullOrWhiteSpace(OrderExpression))
				{
					sb.Append(FormatClause(Clause.OrderBy, OrderExpression));
				}

				return sb.ToString().Trim();
			}
		}

		public static SqlQueryBuilder Select(String selectClause)
		{
			SqlQueryBuilder sql = new SqlQueryBuilder() { SelectExpession = selectClause };
			return sql;
		}

		public SqlQueryBuilder From(String fromClause)
		{
			FromExpression = fromClause;
			return this;
		}
		public SqlQueryBuilder From(String fromClause, String alias)
		{
			FromExpression = $"{fromClause} {alias}";
			return this;
		}
		public SqlQueryBuilder From(SqlQueryBuilder from, String alias)
		{
			FromExpression = $"({from.Sql}) {alias}";
			return this;
		}

		public SqlQueryBuilder Where(String whereExpression)
		{
			if (WhereExpression == null)
			{
				WhereExpression = new WhereExpression(whereExpression);
			}
			else
			{
				WhereExpression = WhereExpression.And(whereExpression);
			}
			return this;
		}
		public SqlQueryBuilder Where(IBooleanExpression expression)
		{
			if (WhereExpression == null)
			{
				WhereExpression = expression;
			}
			else
			{
				WhereExpression = WhereExpression.And(expression);
			}
			return this;
		}

		public SqlQueryBuilder GroupBy(String groupByClause)
		{
			this.GroupExpression = groupByClause;
			return this;
		}

		public SqlQueryBuilder OrderBy(String orderByClause)
		{
			this.OrderExpression = orderByClause;
			return this;
		}
	}

	public enum ExpressionType
	{
		And,
		Or
	}

	public interface IBooleanExpression
	{
		String Expression { get; }
		ExpressionType Type { get; }
	}

	public class WhereExpression : IBooleanExpression
	{
		public static String AndClause { get; private set; } = "AND";
		public static String OrClause { get; private set; } = "OR";

		private readonly String expression;
		public String Expression => expression;
		public ExpressionType Type { get; private set; }
		public bool Negated { get; }

		public WhereExpression(String expression)
		{
			if (String.IsNullOrWhiteSpace(expression))
			{
				throw new ArgumentException("Требуется непустое выражение", nameof(expression));
			}
			this.expression = expression;
			Type = ExpressionType.And;
		}
		public WhereExpression(IEnumerable<String> expressions, ExpressionType type = ExpressionType.And)
		{
			String operation;
			switch (type)
			{
				case ExpressionType.And:
					operation = AndClause;
					break;
				case ExpressionType.Or:
					operation = OrClause;
					break;
				default:
					operation = AndClause;
					break;
			}
			String.Join($" {operation} ", expressions);
			Type = type;
		}

	}

	public static class WhereExtension
	{
		private static String OperationClause(this ExpressionType type)
		{
			switch (type)
			{
				case ExpressionType.And:
					return WhereExpression.AndClause;
				case ExpressionType.Or:
					return WhereExpression.OrClause;
				default:
					return WhereExpression.AndClause;
			}
		}

		private static WhereExpression Operation(IBooleanExpression leftExpr, IBooleanExpression rightExpr, ExpressionType operation)
		{
			return new WhereExpression($"{WrapInBrackets(leftExpr, operation)} {operation.OperationClause()} {WrapInBrackets(rightExpr, operation)}");
		}
		private static WhereExpression Operation(IBooleanExpression leftExpr, String rightExpr, ExpressionType operation)
		{
			return new WhereExpression($"{WrapInBrackets(leftExpr, operation)} {operation.OperationClause()} {WrapInBrackets(rightExpr, operation)}");
		}

		public static IBooleanExpression And(this IBooleanExpression leftExpr, IBooleanExpression expression)
		{
			return Operation(leftExpr, expression, ExpressionType.And);
		}
		public static IBooleanExpression And(this IBooleanExpression leftExpr, String expression)
		{
			return Operation(leftExpr, expression, ExpressionType.And);
		}
		public static IBooleanExpression Or(this IBooleanExpression leftExpr, IBooleanExpression expression)
		{
			return Operation(leftExpr, expression, ExpressionType.Or);
		}
		public static IBooleanExpression Or(this IBooleanExpression leftExpr, String expression)
		{
			return Operation(leftExpr, expression, ExpressionType.Or);
		}

		private static String WrapInBrackets(IBooleanExpression expr, ExpressionType type)
		{
			return $"({expr.Expression})";
		}
		private static String WrapInBrackets(String expr, ExpressionType type)
		{
			return $"({expr})";
		}

	}
}
