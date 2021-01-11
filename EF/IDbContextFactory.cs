using System;
using System.Data.Entity;

namespace Kovi.Data.EF
{
	/// <summary>
	/// Поставщик контекстов для EntityFramework
	/// </summary>
	public interface IDbContextFactory
	{
		DbContext Create(String connection = null, String context = null);
	}



}
