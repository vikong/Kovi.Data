using System;

using Kovi.LinqExtensions;

namespace Kovi.Data.Cqrs
{
	/// <summary>
	/// Представляет параметры запроса
	/// </summary>
	public interface IQriteria { }

	/// <summary>
	/// Представляет строку соединения с поставщиком данных
	/// </summary>
	public interface IConnection
	{ 
		String Connection { get; }
	}

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