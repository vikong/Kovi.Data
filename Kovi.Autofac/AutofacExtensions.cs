using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;

namespace Kovi.Autofac
{
	public static class AutofacExtensions
	{
		/// <summary>
		/// Регистрирует перечисленные типы с их интерфейсами в контейнере IoC
		/// </summary>
		/// <param name="builder">Autofac.Builder</param>
		/// <param name="servicesToRegister">Типы для регистрации</param>
		/// <param name="assembliesToScan">Сборки для сканирования</param>
		/// <returns>Autofac.Builder</returns>
		public static ContainerBuilder RegisterAllServices(this ContainerBuilder builder,
			IEnumerable<Type> servicesToRegister,
			IEnumerable<Assembly> assembliesToScan)
		{
			// все типы в сборках
			TypeInfo[] allTypes = (assembliesToScan as Assembly[] ?? assembliesToScan.ToArray())
						  .Where(a => !a.IsDynamic)
						  .Distinct()
						  .SelectMany(a => a.DefinedTypes)
						  .ToArray();

			Type[] services = servicesToRegister as Type[] ?? servicesToRegister.ToArray();

			// Generic interfaces
			var typesForRegister = services.Where(s => !s.ContainsGenericParameters)
				.SelectMany(
					service => allTypes.Where(
						t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(service)
					)
				);

			foreach (var type in services.Where(s => s.ContainsGenericParameters)
				.SelectMany(
					service => allTypes.Where(
						t => t.IsClass && !t.IsAbstract && ImplementsGenericInterface(t.AsType(), service)
					)
				))
			{
				builder.RegisterGeneric(type.AsType()).AsImplementedInterfaces().SingleInstance();
			}

			// Simple interfaces
			foreach (var type in services.Where(s => !s.ContainsGenericParameters)
				.SelectMany(
					service => allTypes.Where(
						t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(service)
					))
				)
			{
				builder.RegisterType(type.AsType()).AsImplementedInterfaces().SingleInstance();
			}

			return builder;
		}

		private static bool ImplementsInterface(Type type, Type interfaceType)
			=> type.GetTypeInfo().ImplementedInterfaces.Any(i => IsGenericType(i, interfaceType));

		private static bool ImplementsGenericInterface(Type type, Type interfaceType)
			=> IsGenericType(type, interfaceType) || type.GetTypeInfo().ImplementedInterfaces.Any(i => IsGenericType(i, interfaceType));

		private static bool IsGenericType(Type type, Type genericType)
				   => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericType;
	}
}