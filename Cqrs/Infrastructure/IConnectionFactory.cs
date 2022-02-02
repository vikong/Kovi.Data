namespace Kovi.Data.Cqrs
{
	using System;
	using System.Data;

	/// <summary>
	/// Фабрика для создания <see cref="IDbConnection" />
	/// </summary>
	public interface IConnectionFactory
	{
		/// <summary>
		/// Создает <see cref="IDbConnection"/>
		/// </summary>
		IDbConnection Create(String connectionString = null);
	}
}