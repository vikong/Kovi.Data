using System;

using Kovi.LinqExtensions;

namespace Kovi.Data.Cqrs
{
	/// <summary>
	/// Интерфейс, сигнализирующий о том, что класс является критерием для запроса
	/// </summary>
	public interface IQriteria { }

	public interface IPageQriteria 
		: IQriteria
		, IPaging
	{
		String OrderBy { get; set; }
	}

	public interface IPageQriteria<T>
		: IQriteria
		, IPaging
	{
		T Subject { get; set; }
		String OrderBy { get; set; }
	}
}