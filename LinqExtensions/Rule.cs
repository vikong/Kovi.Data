using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Kovi.LinqExtensions.Expressions;

namespace Kovi.LinqExtensions.Specification
{
	/// <summary>
	/// Представляет предикат для описания фильтрации данных. Поддерживает композицию в стиле логических операторов.
	/// </summary>
	public class Rule<T>
		: IEntitySpecification<T>
		where T : class
	{

		public Rule(Expression<Func<T, Boolean>> expression)
		{
			Expression = expression;
		}

		public Expression<Func<T, Boolean>> Expression { get; }

		public Func<T, Boolean> Func => Expression.AsFunc();

		public Boolean IsSatisfiedBy(T item)
			=> Satisfied( new T[] { item }.AsQueryable() ).Any();

		public virtual IQueryable<T> Satisfied(IQueryable<T> candidates) 
			=> candidates.Where(Expression);

		#region Predicate Booleans

		/// <summary>
		/// Creates a predicate that evaluates to true.
		/// </summary>
		public static Rule<T> True 
			= Create(param => true);

		/// <summary>
		/// Creates a predicate that evaluates to false.
		/// </summary>
		public static Rule<T> False 
			= Create(param => false);

		#endregion

		#region Factories

		/// <summary>
		/// Creates a predicate expression from the specified lambda expression.
		/// </summary>
		public static Rule<T> Create(Expression<Func<T, Boolean>> expression)
			=> new Rule<T>(expression);

		public static Rule<T> Create<TPredicate>(Expression<Func<TPredicate, Boolean>> predicateExpression, Expression<Func<T, TPredicate>> convertExpression)
			=> new Rule<T>(convertExpression.Predicated(predicateExpression));

		public static Rule<T> Create<TPredicate>(Rule<TPredicate> predicateRule, Expression<Func<T, TPredicate>> convertExpression)
			where TPredicate : class
			=> new Rule<T>(convertExpression.Predicated(predicateRule));

		public static Rule<T> InList<TList>(IEnumerable<TList> list, Expression<Func<T, TList>> convertExpression)
			where TList : class
			=> new Rule<T>(PredicateHelper.In<T, TList>(convertExpression, list));

		#endregion Factories

		public Rule<TIn> For<TIn>(Expression<Func<TIn, T>> convertExpression)
			where TIn : class
			=> Rule<TIn>.Create(convertExpression.Predicated(Expression));

		/// <summary>
		/// Combines the first predicate with the second using the logical "and".
		/// </summary>
		public Rule<T> And(Expression<Func<T, Boolean>> expression)
			=> Create(this.Expression.And(expression)); //Compose(expression, System.Linq.Expressions.Expression.AndAlso));

		public Rule<T> And(Rule<T> predicate) 
			=> And(predicate.Expression);

		/// <summary>
		/// Combines the first predicate with the second using the logical "or".
		/// </summary>
		public Rule<T> Or(Expression<Func<T, Boolean>> expression) 
			=> Create(Expression.Or(expression)); // Compose(expression, System.Linq.Expressions.Expression.OrElse));

		public Rule<T> Or(Rule<T> rule) 
			=> Or(rule.Expression);

		/// <summary>
		/// Negates the predicate.
		/// </summary>
		public Rule<T> Not() 
			=> Create(this.Expression.Not());

		#region Implicit conversion to and from Expression<Func<TExpressionFuncType, Boolean>>

		public static implicit operator Rule<T>(Expression<Func<T, Boolean>> expression) 
			=> Create(expression);

		public static implicit operator Expression<Func<T, Boolean>>(Rule<T> predicate) 
			=> predicate.Expression;

		#endregion

		#region Operator Overloads

		public static Rule<T> operator !(Rule<T> predicate) 
			=> predicate.Not();

		public static Rule<T> operator &(Rule<T> first, Rule<T> second) 
			=> first.And(second);

		public static Rule<T> operator |(Rule<T> first, Rule<T> second) 
			=> first.Or(second);

		//Both should return false so that Short-Circuiting (Conditional Logical Operator ||)
		public static Boolean operator true(Rule<T> first) => false;
		public static Boolean operator false(Rule<T> first) => false;
		
		#endregion

	}

	public static class RuleExtensions
	{

		public static Rule<T> With<T, TPredicate>(this Expression<Func<T, TPredicate>> converterExpr, Expression<Func<TPredicate, Boolean>> predicateExpr)
			where T:class
			=> Rule<T>.Create(converterExpr.Predicated(predicateExpr));

		public static Rule<T> AndIf<T>(this Rule<T> rule, Boolean condition, Expression<Func<T, Boolean>> expression)
			where T : class
			=> condition ? Rule<T>.Create(rule.Expression.And(expression)) : rule; //Compose(expression, System.Linq.Expressions.Expression.AndAlso));

		public static Boolean Is<T>(this T entity, Rule<T> rule)
			where T : class	
			=> rule.Func(entity);

	}

}
