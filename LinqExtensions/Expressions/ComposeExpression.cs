using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace Kovi.LinqExtensions.Expressions
{
	public static class ComposeExpressionExtensions
	{
		// Compose: [ y => f(x) ].Then[ z => g(y) ] -> x => g( f(x) )
		/// <summary>
		/// Composes two Lambdas: [ y => f(x) and z => g(y) ] into Lambda [ z => g( f(x) ) ]
		/// </summary>
		/// <param name="Tpg">Type of parameter to gFn, and type of parameter to result lambda.</param>
		/// <param name="Tpf">Type of result of gFn and type of parameter to fFn.</param>
		/// <param name="TRes">Type of result of fFn and type of result of result lambda.</param>
		/// <param name="fFn">The outer LambdaExpression.</param>
		/// <param name="gFn">The inner LambdaExpression.</param>
		/// <returns>LambdaExpression representing outer composed with inner</returns>
		public static Expression<Func<T1, T3>> Then<T1, T2, T3>(this Expression<Func<T1, T2>> firstExpr, Expression<Func<T2, T3>> thenExpr )
			=> Expression.Lambda<Func<T1, T3>>(thenExpr.Body.Replace(thenExpr.Parameters[0], firstExpr.Body), firstExpr.Parameters[0]);


		/// <summary>
		/// Replaces an Expression (reference Equals) with another Expression
		/// </summary>
		/// <param name="orig">The original Expression.</param>
		/// <param name="from">The from Expression.</param>
		/// <param name="to">The to Expression.</param>
		/// <returns>Expression with all occurrences of from replaced with to</returns>
		public static Expression Replace(this Expression orig, Expression from, Expression to) 
			=> new ReplaceVisitor(from, to).Visit(orig);

		public static Expression PropagateNull(this Expression orig)
			=> new NullVisitor().Visit(orig);

		// Apply: (x => f).Apply(args)
		/// <summary>
		/// Substitutes an array of Expression args for the parameters of a lambda, returning a new Expression
		/// </summary>
		/// <param name="expr">The original LambdaExpression to "call".</param>
		/// <param name="args">The Expression[] of values to substitute for the parameters of e.</param>
		/// <returns>Expression representing e.Body with args substituted in</returns>
		public static Expression Apply(this LambdaExpression expr, params Expression[] args)
		{
			var b = expr.Body;

			foreach (var pa in expr.Parameters.Zip(args, (p, a) => (p, a)))
				b = b.Replace(pa.p, pa.a);

			return b.PropagateNull();
		}

	}

	/// <summary>
	/// ExpressionVisitor to replace an Expression (that is Equals) with another Expression.
	/// </summary>
	public class ReplaceVisitor : ExpressionVisitor
	{
		private readonly Expression from;
		private readonly Expression to;

		public ReplaceVisitor(Expression from, Expression to)
		{
			this.from = from;
			this.to = to;
		}

		public override Expression Visit(Expression node) 
			=> node == from ? to : base.Visit(node);
	}

	/// <summary>
	/// ExpressionVisitor to replace a null.member Expression with a null
	/// </summary>
	public class NullVisitor : ExpressionVisitor
	{
		public override Expression Visit(Expression node)
		{
			if (node is MemberExpression nme && nme.Expression is ConstantExpression nce && nce.Value == null)
				return Expression.Constant(null, nce.Type.GetMember(nme.Member.Name).Single().GetMemberType());
			else
				return base.Visit(node);
		}
	}

	public static class MeberInfoExt
	{
		public static Type GetMemberType(this MemberInfo member)
		{
			switch (member)
			{
				case FieldInfo mfi:
					return mfi.FieldType;

				case PropertyInfo mpi:
					return mpi.PropertyType;

				case EventInfo mei:
					return mei.EventHandlerType;

				default:
					throw new ArgumentException("MemberInfo must be if type FieldInfo, PropertyInfo or EventInfo", nameof(member));
			}
		}
	}

}
