using System;
using System.Linq;
using System.Linq.Expressions;

namespace Kovi.LinqExtensions.Specification
{
	/// <summary>
	/// Спецификация для фильтрации выборки
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface ISpecification<T>
	{
		Expression< Func<T, Boolean> > Expression { get; }
	}

	public interface IEntitySpecification<T>: ISpecification<T>
	{
		IQueryable<T> Satisfied(IQueryable<T> candidates);
		Boolean IsSatisfiedBy(T item);
	}

}
