using System;
using System.Threading.Tasks;

namespace Kovi.Data.Cqrs
{
	/// <summary>
	/// Представляет запрос, возвращающий данные по параметру
	/// </summary>
	/// <typeparam name="TParam">Критерий</typeparam>
	/// <typeparam name="TResponse">Результат выполнения запроса</typeparam>
	public interface IQuery<in TParam, TResponse>
		where TParam : IQriteria
	{
		TResponse Ask(TParam qrit, String source=null);
		Task<TResponse> AskAsync(TParam qrit, String source=null);
	}

	public interface IByIdQuery<TResponse>
		: IQuery<IHasId, TResponse>
	{ }
}