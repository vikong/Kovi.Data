using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using AutoMapper;

using Kovi.Data.Cqrs.Infrastructure;

namespace Kovi.Data.Automapper
{
	/// <summary>
	/// Модуль автоматического маппирования сущностей, снабженных атрибутом EntityMapAttribute
	/// </summary>
	public class AutoMapAttributeProfile : Profile
	{
		public IDictionary<Type, Type[]> TypeMap;

		public AutoMapAttributeProfile(params Assembly[] assemblies)
		{
			if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));

			TypeMap = assemblies
				.SelectMany(x => x.GetTypes())
				.Where(x => x.GetTypeInfo().GetCustomAttribute<EntityMapAttribute>() != null)
				.GroupBy(x => x.GetTypeInfo().GetCustomAttribute<EntityMapAttribute>().EntityType)
				.ToDictionary(k => k.Key, v => v.ToArray());

			foreach (var kv in TypeMap)
			{
				foreach (var v in kv.Value)
				{
					var attr = v.GetTypeInfo().GetCustomAttribute<EntityMapAttribute>();

					if (attr.MapOptions.HasFlag(MapOptions.EntityToDto))
					{
						CreateMap(kv.Key, v);
					}

					if (attr.MapOptions.HasFlag(MapOptions.DtoToEntity))
					{
						var hasParameterlessCtor = kv.Key
							.GetTypeInfo()
							.DeclaredConstructors.Any(x => !x.GetParameters().Any());

						if (!hasParameterlessCtor)
							throw new InvalidOperationException(
								$"Type {kv.Key} should provide parameterless constructor (public, protected or private) in order to be mapped");

						CreateMap(v, kv.Key);
						//	.ConvertUsing(typeof(DtoToEntityTypeConverter<,,>)
						//		.MakeGenericType(kv.Key.GetTypeInfo().GetProperty("Id").PropertyType, v, kv.Key));
					}
				}
			}
		}
	}

}
