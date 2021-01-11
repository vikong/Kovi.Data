using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Kovi.LinqExtensions.Expressions;

namespace Kovi.Data.Cqrs.Infrastructure
{
	public static class ConventionBuilder<TSubject>
	{
		public static Expression<Func<TSubject, Boolean>> IdFilterExpression(IHasId qrit)
		{
			//	прямой Where(e => e.Id == qrit.Id) - не работает с Linq2Object, так как проверяется Object.Equal

			ParameterExpression pe = Expression.Parameter(typeof(TSubject), "entity");

			Expression predicate = Expression.Equal(
				Expression.Property(pe, Conventions.IdProp),
				Expression.Constant(qrit.Id)
			);

			return Expression.Lambda<Func<TSubject, Boolean>>(predicate, pe);
		}

		public static Expression<Func<TSubject, Boolean>> FilterExpression<TPredicate>(TPredicate predicate,
			Conventions.ComposeKind composeKind = Conventions.ComposeKind.And
			)
		{
			if (predicate == null) throw new ArgumentNullException(nameof(predicate));

			// ищем беспараметрический конструктор
			var defaultPredicate = FastTypeInfo<TPredicate>.Constructors
				.Any(c => c.GetParameters().Length == 0) ?
				Activator.CreateInstance(typeof(TPredicate))
				: null;

			// ***сравнение значений полей
			Boolean Equal(PropertyInfo pi, Object a, Object b)
			{
				if (a == null || b == null)
					return false;
				var aVal = pi.GetValue(a);
				var bVal = pi.GetValue(b);
				return aVal != null ? aVal.Equals(bVal) : bVal != null ? false : true;
			}
			// *** end Equal

			// фильтруем только по тем полям, значения которых отличны от дефолтных
			var filterProps = FastTypeInfo<TPredicate>
				.PublicProperties
				.Where(x => !Equal(x, predicate, defaultPredicate))
				.ToArray();

			var filterPropNames = filterProps
				.Select(x => x.Name)
				.ToArray();

			var modelType = typeof(TSubject);

			var parameter = Expression.Parameter(modelType);

			var props = FastTypeInfo<TSubject>
				.PublicProperties
				.Where(x => filterPropNames.Contains(x.Name))
				.Select(x => new
				{
					Property = x,
					Value = filterProps.Single(y => y.Name == x.Name).GetValue(predicate)
				})
				.Where(x => x.Value != null)
				.Select(x =>
				{
					var property = Expression.Property(parameter, x.Property);
					Expression value = Expression.Constant(x.Value);

					value = Expression.Convert(value, property.Type);
					var body = Conventions.Filters[property.Type](property, value);

					return Expression.Lambda<Func<TSubject, Boolean>>(body, parameter);
				})
				.ToArray();

			if (!props.Any())
			{
				return Expression.Lambda<Func<TSubject, Boolean>>(Expression.Constant(true), parameter);
			}

			var expr = composeKind == Conventions.ComposeKind.And
				? props.Aggregate((c, n) => c.And(n))
				: props.Aggregate((c, n) => c.Or(n));

			return expr;
		}

		public static Expression<Func<TSubject, Object>> SortExpression(String propertyName)
		{
			var property = FastTypeInfo<TSubject>
				.PublicProperties
				.FirstOrDefault(x => String.Equals(x.Name, propertyName, StringComparison.CurrentCultureIgnoreCase));

			if (property == null)
				throw new InvalidOperationException($"There is no public property \"{propertyName}\" " +
													$"in type \"{typeof(TSubject)}\"");

			var parameter = Expression.Parameter(typeof(TSubject));
			var body = Expression.Property(parameter, propertyName);

			var lambda = FastTypeInfo<Expression>
				.PublicMethods
				.First(x => x.Name == "Lambda");

			lambda = lambda.MakeGenericMethod(typeof(Func<,>)
				.MakeGenericType(typeof(TSubject), property.PropertyType));

			return Expression.Lambda<Func<TSubject, Object>>(body, parameter);
		}
	}
}