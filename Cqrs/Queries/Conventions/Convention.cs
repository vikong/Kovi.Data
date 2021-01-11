using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Kovi.Data.Cqrs.Infrastructure
{
	public static class Conventions
	{
		public static ConventionalFilters Filters { get; } = new ConventionalFilters();

		public enum ComposeKind
		{
			And, Or
		}

		public static readonly String IdProp = "Id";
		public static readonly String OrderProp = "OrderBy";
	}

	public class ConventionalFilters
	{
		private static readonly MethodInfo StartsWith = typeof(String)
			.GetMethod("StartsWith", new[] { typeof(String) });

		private static readonly Dictionary<Type, Func<MemberExpression, Expression, Expression>> _filters
			= new Dictionary<Type, Func<MemberExpression, Expression, Expression>>()
			{
				{ typeof(String),  (p, v) => Expression.Call(p, StartsWith, v) }
			};

		internal ConventionalFilters()
		{ }

		public Func<MemberExpression, Expression, Expression> this[Type key]
		{
			get => _filters.ContainsKey(key)
				? _filters[key]
				: Expression.Equal;

			set => _filters[key] = value ?? throw new ArgumentException(nameof(value));
		}
	}
}