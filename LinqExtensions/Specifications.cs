using System;
using System.Linq;
using System.Linq.Expressions;
using Kovi.LinqExtensions.Expressions;

namespace Kovi.LinqExtensions.Specification
{

	/// <summary>
	/// Класс спецификации для реализаций операций AND, OR, NOT для спецификаций
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class Specification<T> : ISpecification<T>
	{
		private readonly ISpecification<T> _leftSpec;
		private readonly ISpecification<T> _rightSpec;
		protected Specification(ISpecification<T> leftSpec, ISpecification<T> rightSpec)
		{
			_leftSpec = leftSpec ?? throw new ArgumentNullException(nameof(leftSpec));
			_rightSpec = rightSpec ?? throw new ArgumentNullException(nameof(rightSpec));
		}

		protected ISpecification<T> LeftSpec => _leftSpec;

		protected ISpecification<T> RightSpec => _rightSpec;

		public abstract Expression<Func<T, Boolean>> Expression { get; }
	} 

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal class AndSpecification<T> : Specification<T>, ISpecification<T>
    {
		public AndSpecification(ISpecification<T> leftSpec, ISpecification<T> rightSpec)
			: base(leftSpec, rightSpec) { }

		public override Expression<Func<T, Boolean>> Expression
			=> LeftSpec.Expression.And(RightSpec.Expression);
	}
 
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal class OrSpecification<T> : Specification<T>, ISpecification<T>
    {
		public OrSpecification(ISpecification<T> leftSpec, ISpecification<T> rightSpec)
			: base(leftSpec, rightSpec) { }

		public override Expression<Func<T, Boolean>> Expression 
			=> LeftSpec.Expression.Or(RightSpec.Expression);
	}
 
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
    internal class NotSpecification<T> : ISpecification<T>
    {
		protected ISpecification<T> Wrapped { get; }

		internal NotSpecification(ISpecification<T> spec)
        {
			Wrapped = spec ?? throw new ArgumentNullException(nameof(spec));
        }

		public Expression<Func<T, Boolean>> Expression 
			=> Wrapped.Expression.Not();
	}

	/// <summary>
	/// Расширения AND, OR, NOT для спецификаций
	/// </summary>
	public static class ExtensionMethods
	{
		public static ISpecification<T> And<T>(this ISpecification<T> spec1, ISpecification<T> spec2) 
			=> new AndSpecification<T>(spec1, spec2);

		public static ISpecification<T> Or<T>(this ISpecification<T> spec1, ISpecification<T> spec2) 
			=> new OrSpecification<T>(spec1, spec2);

		public static ISpecification<T> Not<T>(this ISpecification<T> spec) 
			=> new NotSpecification<T>(spec);
	}

	public static class QuerySpecificationExtension
	{
		public static IQueryable<T> WhereSpec<T>(this IQueryable<T> source, ISpecification<T> spec) 
			=> source.Where(spec.Expression);

		public static IQueryable<T> Where<T>(this IQueryable<T> source, ISpecification<T> spec) 
			=> source.Where(spec.Expression);
	}

}
