using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Kovi.Data.Cqrs.Infrastructure
{
	internal delegate T ObjectActivator<out T>(params Object[] args);

	public static class FastTypeInfo<T>
	{
		private static readonly ConstructorInfo[] _constructors
			= typeof(T).GetConstructors();

		private static readonly ConcurrentDictionary<String, ObjectActivator<T>> _activators
			= new ConcurrentDictionary<String, ObjectActivator<T>>();

		public static IEnumerable<PropertyInfo> PublicProperties { get; }
			= typeof(T).GetProperties().Where(x => x.CanRead && x.CanWrite).ToArray();

		public static IEnumerable<MethodInfo> PublicMethods { get; }
			= typeof(T).GetMethods().Where(x => x.IsPublic && !x.IsAbstract).ToArray();

		public static IEnumerable<ConstructorInfo> Constructors { get => _constructors; }

		public static IEnumerable<Attribute> Attributes { get; }
			= typeof(T).GetCustomAttributes().ToArray();

		public static Boolean HasAttribute<TAttr>()
			where TAttr : Attribute
			=> Attributes.Any(x => x.GetType() == typeof(TAttr));

		public static TAttr GetCustomAttribute<TAttr>()
			where TAttr : Attribute
			=> (TAttr)Attributes.FirstOrDefault(x => x.GetType() == typeof(TAttr));

		#region Create

		public static T Create(params Object[] args)
			=> _activators.GetOrAdd(
				GetSignature(args),
				GetActivator(GetConstructorInfo(args)))
					.Invoke(args);

		private static String GetSignature(Object[] args)
			=> String.Join(",",
				args.Select(x => x.GetType().ToString())
				);

		private static ConstructorInfo GetConstructorInfo(Object[] args)
		{
			for (var i = 0; i < _constructors.Length; i++)
			{
				var consturctor = _constructors[i];
				var ctrParams = consturctor.GetParameters();
				if (ctrParams.Length != args.Length)
				{
					continue;
				}

				var flag = true;
				for (var j = 0; j < args.Length; i++)
				{
					if (ctrParams[j].ParameterType != args[j].GetType())
					{
						flag = false;
						break;
					}
				}

				if (!flag)
				{
					continue;
				}

				return consturctor;
			}

			var signature = GetSignature(args);

			throw new InvalidOperationException(
				$"Constructor ({signature}) is not found for {typeof(T)}");
		}

		private static ObjectActivator<T> GetActivator(ConstructorInfo ctor)
		{
			//var type = ctor.DeclaringType;
			var paramsInfo = ctor.GetParameters();

			//create a single param of type Object[]
			var param = Expression.Parameter(typeof(Object[]), "args");

			var argsExp = new Expression[paramsInfo.Length];

			//pick each arg from the params array
			//and create a typed expression of them
			for (var i = 0; i < paramsInfo.Length; i++)
			{
				var index = Expression.Constant(i);
				var paramType = paramsInfo[i].ParameterType;

				Expression paramAccessorExp = Expression.ArrayIndex(param, index);
				Expression paramCastExp = Expression.Convert(paramAccessorExp, paramType);

				argsExp[i] = paramCastExp;
			}

			//make a NewExpression that calls the
			//ctor with the args we just created
			var newExp = Expression.New(ctor, argsExp);

			//create a lambda with the New
			//Expression as body and our param Object[] as arg
			var lambda = Expression.Lambda(typeof(ObjectActivator<T>), newExp, param);

			//compile it
			var compiled = (ObjectActivator<T>)lambda.Compile();
			return compiled;
		}

		public static Delegate CreateMethod(MethodInfo method)
		{
			if (method == null)
			{
				throw new ArgumentNullException(nameof(method));
			}

			if (!method.IsStatic)
			{
				throw new ArgumentException("The provided method must be static.", nameof(method));
			}

			if (method.IsGenericMethod)
			{
				throw new ArgumentException("The provided method must not be generic.", nameof(method));
			}

			var parameters = method.GetParameters()
				.Select(p => Expression.Parameter(p.ParameterType, p.Name))
				.ToArray();

			var call = Expression.Call(null, method, parameters);
			return Expression.Lambda(call, parameters).Compile();
		}

		#endregion Create

		public static Func<TObject, TProperty> PropertyGetter<TObject, TProperty>(String propertyName)
		{
			var paramExpression = Expression.Parameter(typeof(TObject), "value");

			var propertyGetterExpression = Expression.Property(paramExpression, propertyName);

			var result = Expression.Lambda<Func<TObject, TProperty>>(propertyGetterExpression, paramExpression)
				.Compile();

			return result;
		}

		public static Action<TObject, TProperty> PropertySetter<TObject, TProperty>(String propertyName)
		{
			var paramExpression = Expression.Parameter(typeof(TObject));
			var paramExpression2 = Expression.Parameter(typeof(TProperty), propertyName);
			var propertyGetterExpression = Expression.Property(paramExpression, propertyName);
			var result = Expression.Lambda<Action<TObject, TProperty>>
			(
				Expression.Assign(propertyGetterExpression, paramExpression2), paramExpression, paramExpression2
			).Compile();

			return result;
		}
	}
}