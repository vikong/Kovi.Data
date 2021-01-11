using System;
using System.Linq;
using System.Linq.Expressions;

namespace Kovi.Data.Cqrs.Linq
{
	public interface IFilterSpec<TParam, TEntity>
		where TEntity : class
	{
		Expression<Func<TEntity, Boolean>> Expression(TParam qrit);
	}

	public class ExpressionFilterSpec<TParam, TEntity>
		: IFilterSpec<TParam, TEntity>
		, ILinqSpec<TParam,TEntity>
		where TEntity : class
	{
		private readonly Expression<Func<TEntity, Boolean>> _expression;

		public ExpressionFilterSpec(Expression<Func<TEntity, Boolean>> expression)
		{
			_expression = expression;
		}

		public Expression<Func<TEntity, Boolean>> Expression(TParam qrit)
			=> _expression;

		public IQueryable<TEntity> Query(IQueryable<TEntity> query, TParam qrit) 
			=> query.Where(Expression(qrit));
	}

}