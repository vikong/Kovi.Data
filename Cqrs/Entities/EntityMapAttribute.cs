using System;

namespace Kovi.Data.Cqrs.Infrastructure
{
	/// <summary>
	/// Тип проекции
	/// </summary>
	[Flags]
	public enum MapOptions
	{
		EntityToDto = 0x0,
		DtoToEntity = 0x1
	}

	/// <summary>
	/// Декларирует проекцию данных на другой тип.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class EntityMapAttribute : Attribute
	{
		public Type EntityType { get; }

		public MapOptions MapOptions { get; }

		public Type ConventionQuery { get; }

		/// <summary>
		/// Инициализирует новый экземпляр атрибута с указанием типа - источника.
		/// </summary>
		/// <param name="entityType">Тип источник</param>
		/// <param name="mapType">Тип поля</param>
		/// <param name="conventionQuery">запрос по конвенции</param>
		public EntityMapAttribute(Type entityType, MapOptions mapOptions = MapOptions.EntityToDto, Type conventionQuery = null)
		{
			EntityType = entityType;
			MapOptions = mapOptions;
			ConventionQuery = conventionQuery;
		}
	}
}