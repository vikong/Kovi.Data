using System.Threading.Tasks;

namespace Kovi.Data.Cqrs
{
	/// <summary>
	/// Представляет возможности обработки входных данных с возвратом обработанных данных
	/// </summary>
	/// <typeparam name="TIn">Тип входных данных</typeparam>
	/// <typeparam name="TOut">Тип возвращаемых данных</typeparam>
	public interface IHandler<in TIn, TOut>
	{
		/// <summary>
		/// Обрабатывает входные данные
		/// </summary>
		/// <param name="param">Входные данные</param>
		/// <returns>Результат обработки</returns>
		TOut Handle(TIn param);
	}

	/// <summary>
	/// Представляет возможности асинхронной обработки входных данных с возвратом обработанных данных
	/// </summary>
	/// <typeparam name="TIn">Тип входных данных</typeparam>
	/// <typeparam name="TOut">Тип возвращаемых данных</typeparam>
	public interface IAsyncHandler<in TIn, TOut>
	{
		/// <summary>
		/// Обрабатывает входные данные
		/// </summary>
		/// <param name="param">Входные данные</param>
		/// <returns>Ожидаемый результат обработки</returns>
		Task<TOut> HandleAsync(TIn param);
	}
}