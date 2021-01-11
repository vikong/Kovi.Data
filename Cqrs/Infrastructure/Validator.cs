using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Kovi.Data.Cqrs
{
	/// <summary>
	/// Указывает на то, что выполнение команды предваряется проверочным запросом.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class ValidateCommandAttribute : Attribute
	{
		public Type ValidateQueryType { get; }

		public static readonly Type QueryType = typeof(IQuery<,>);

		public Boolean IsValid { get => QueryType.IsAssignableFrom(ValidateQueryType); }

		public ValidateCommandAttribute(Type validateQueryType)
		{
			ValidateQueryType = validateQueryType ?? throw new ArgumentNullException(nameof(validateQueryType));
		}
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public abstract class ValidateAttribute : Attribute
	{
		public abstract IEnumerable<ValidationResult> Validate(Object context);
	}

	public static class ValidatorExtension
	{
		public static Boolean Validate(this Object obj, out IEnumerable<ValidationResult> validationResults)
		{
			var context = new ValidationContext(obj);
			var res = new List<ValidationResult>();
			Validator.TryValidateObject(obj, context, res, true);

			// Проверяем атрибуты
			var validator = obj.GetType()
				.GetCustomAttributes<ValidateAttribute>()
				.FirstOrDefault();

			if (validator != null)
				res.AddRange(validator.Validate(obj));

			validationResults = res;

			return res.Count == 0;
		}
	}
}