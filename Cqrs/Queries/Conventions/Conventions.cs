using System;
using System.Linq;
using System.Linq.Expressions;

using Kovi.Data.Cqrs.Infrastructure;
using Kovi.LinqExtensions;
using Kovi.LinqExtensions.Expressions;

namespace Kovi.Data.Cqrs.Linq
{
	public static class Conventions<TSubject>
		where TSubject : class
	{
		private static Type _entityType;

		public static Type EntityType
		{
			get
			{
				if (_entityType != null)
					return _entityType;

				_entityType = FastTypeInfo<TSubject>
					.Attributes
					.OfType<EntityMapAttribute>()
					.Select(x => x.EntityType)
					.SingleOrDefault();

				if (_entityType == null)
					throw new InvalidOperationException($"Запрашиваемый тип [{typeof(TSubject).FullName}] не привязан к сущности. Используйте AutoMapAttribute для привязки.");

				return _entityType;
			}
		}

		public static ILinqSpec<IHasId> IdSpec
		{
			get
			{
				var idSpec = typeof(ConventionIdLinqSpec<>)
					.MakeGenericType(new Type[] { EntityType });

				return (ILinqSpec<IHasId>)Activator.CreateInstance(idSpec);
			}
		}

		public static ILinqSpec<IPageQriteria<TSubject>> PageSpec()
			=> PageSpec<TSubject>();

		public static ILinqSpec<IPageQriteria<TParam>> PageSpec<TParam>()
		{
			var spec = typeof(ConventionPagedSpec<,>)
				.MakeGenericType(new Type[] { EntityType, typeof(TParam) });

			return (ILinqSpec<IPageQriteria<TParam>>)Activator.CreateInstance(spec);
		}

		public static IOrderedQueryable<TSubject> Sort(IQueryable<TSubject> query, String propertyName)
		{
			//(String, Boolean) GetSorting()
			//{
			//	var arr = propertyName.Split('.');
			//	if (arr.Length == 1)
			//		return (arr[0], false);
			//	var sort = arr[1];
			//	if (String.Equals(sort, "ASC", StringComparison.CurrentCultureIgnoreCase))
			//		return (arr[0], false);
			//	if (String.Equals(sort, "DESC", StringComparison.CurrentCultureIgnoreCase))
			//		return (arr[0], true);
			//	return (arr[0], false);
			//}

			//var (name, isDesc) = GetSorting();
			//propertyName = name;
			var isDesc = false;

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

			var expression = lambda.Invoke(null, new Object[] { body, new[] { parameter } });

			var methodName = isDesc ? "OrderByDescending" : "OrderBy";

			var orderBy = typeof(Queryable)
				.GetMethods()
				.First(x => x.Name == methodName && x.GetParameters().Length == 2)
				.MakeGenericMethod(typeof(TSubject), property.PropertyType);

			return (IOrderedQueryable<TSubject>)orderBy.Invoke(query, new Object[] { query, expression });
		}

		public static IQueryable<TSubject> Filter<TPredicate>(IQueryable<TSubject> query,
			TPredicate predicate,
			Conventions.ComposeKind composeKind = Conventions.ComposeKind.And)
		{
			var filterProps = FastTypeInfo<TPredicate>
				.PublicProperties
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
				return query;
			}

			var expr = composeKind == Conventions.ComposeKind.And
				? props.Aggregate((c, n) => c.And(n))
				: props.Aggregate((c, n) => c.Or(n));

			return query.Where(expr);
		}

		public static IQueryable<TSubject> IdFilter(IQueryable<TSubject> query, IHasId qrit)
			=> query.Where(ConventionBuilder<TSubject>.IdFilterExpression(qrit));

		//Type entityType = typeof(TEntity);
		//ParameterExpression pe = Expression.Parameter(entityType, "entity");
		//Expression myExpr = Expression.Equal(
		//	Expression.Property(pe, HasId.IdProp),
		//	Expression.Constant(qrit.Id)
		//);
		//var exprType = typeof(Func<,>).MakeGenericType(entityType, typeof(Boolean));
		//var predicate = Expression.Lambda(exprType, myExpr, new ParameterExpression[] { pe });
		//MethodCallExpression whereCallExpression = Expression.Call(
		//	typeof(Queryable),
		//	"Where",
		//	new Type[] { entityType },
		//	query.Expression,
		//	predicate//);
		//var result = queryableData.Provider.CreateQuery(whereCallExpression);
	}

	public static class ConventionExtensions
	{
		public static IQueryable<TSubject> OrderByConventions<TSubject, TPredicate>(
			this IQueryable<TSubject> query,
			TPredicate predicate
			)
			where TSubject : class
		{
			var orderBy = FastTypeInfo<TPredicate>
				.PublicProperties
				.FirstOrDefault(x => x.Name == Conventions.OrderProp);

			if (orderBy?.GetValue(predicate, null) is String propertyName)
				return Conventions<TSubject>.Sort(query, propertyName);

			if (typeof(IHasId).IsAssignableFrom(typeof(TSubject)))
				return Conventions<TSubject>.Sort(query, Conventions.IdProp);

			throw new NotImplementedException();
		}

		public static IQueryable<TSubject> AutoFilter<TSubject, TPredicate>(
			this IQueryable<TSubject> query,
			TPredicate predicate,
			Conventions.ComposeKind composeKind = Conventions.ComposeKind.And
			)
			where TSubject : class
				=> Conventions<TSubject>.Filter(query, predicate, composeKind).OrderByConventions(predicate);

		//public static IOrderedQueryable<TSubject> OrderBy<TSubject>(this IQueryable<TSubject> query, String propertyName)
		//	=> Conventions<TSubject>.Sort(query, propertyName);
	}
}