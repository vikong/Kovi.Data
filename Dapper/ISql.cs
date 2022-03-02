using System;

namespace Kovi.Data.Dapper
{
	public interface ISql
	{
		/// <summary>
		/// Запрос SQL для выполнения
		/// </summary>
		String Sql { get; }
	}
}