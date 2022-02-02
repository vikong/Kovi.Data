namespace Kovi.Data.Dapper
{
	using System;

	/// <summary>
	/// Представляет SQL запрос с параметрами
	/// </summary>
	public interface IQueryObject
	{
		/// <summary>
		/// Запрос
		/// </summary>
		String Sql { get; }

		/// <summary>
		/// Параметры запроса
		/// </summary>
		Object QueryParams { get; }
	}
}