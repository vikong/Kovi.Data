using System.Threading.Tasks;

namespace Kovi.Data.Cqrs.Linq
{
	public interface ILinqRequest<TParam, TResponse>
		where TParam : IQriteria
	{
		TResponse Request(ILinqProvider linqProvider, TParam qrit);

		Task<TResponse> RequestAsync(ILinqProvider linqProvider, TParam qrit);
	}

}