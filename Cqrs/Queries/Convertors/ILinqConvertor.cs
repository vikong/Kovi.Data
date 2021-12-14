using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Kovi.LinqExtensions;

namespace Kovi.Data.Cqrs.Linq
{
	/// <summary>
	/// Представляет конвертор из IQueryable в результат запроса.
	/// </summary>
	/// <typeparam name="TResponse">Тип результата.</typeparam>
	public interface ILinqConvertor<TResponse>
	{
		/// <summary>
		/// Конветирует IQueryable в запрашиваемый тип
		/// </summary>
		/// <param name="query">Запрос</param>
		/// <param name="param">Параметры конвертации</param>
		/// <returns>Результат</returns>
		/// <remarks>Параметры конвертации - например, номер и размер страцицы</remarks>
		TResponse Convert(IQueryable query, Object param=null);

		Task<TResponse> ConvertAsync(IQueryable query, Object param=null);

	}

	/// <summary>
	/// Представляет конвертор, который возвращает один объект.
	/// </summary>
	/// <typeparam name="TResponse">тип возвращаемого объекта</typeparam>
	public interface ISingleLinqConvertor<TResponse>
		: ILinqConvertor<TResponse>
	{ }

	/// <summary>
	/// Представляет конвертор, который возвращает множество объектов.
	/// </summary>
	/// <typeparam name="TResponse">тип возвращаемых объектов</typeparam>
	public interface IEnumLinqConvertor<TResponse>
		: ILinqConvertor<IEnumerable<TResponse>>
	{ }

	/// <summary>
	/// Представляет конвертор, который возвращает страницу IPage.
	/// </summary>
	/// <typeparam name="TResponse">тип объектов, содержащихся на странице</typeparam>
	public interface IPageConvertor<TResponse>
		: ISingleLinqConvertor<IPage<TResponse>>
	{ }

	public static class ConvertorExtensions
	{
		public static TResponse Convert<TResponse>(this IQueryable query, ILinqConvertor<TResponse> convertor, Object param=null)
			=> convertor.Convert(query, param);

		public static Task<TResponse> ConvertAsync<TResponse>(this IQueryable query, ILinqConvertor<TResponse> convertor, Object param=null)
			=> convertor.ConvertAsync(query, param);
	}
}