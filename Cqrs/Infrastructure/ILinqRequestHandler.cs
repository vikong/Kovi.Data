using System;
using System.Threading.Tasks;

namespace Kovi.Data.Cqrs.Linq
{
	public interface ILinqRequestHandler
	{
		TResponse Perform<TParam, TResponse>(ILinqRequest<TParam, TResponse> request, TParam qrit, String source = null)
			where TParam : IQriteria;

		Task<TResponse> PerformAsync<TParam, TResponse>(ILinqRequest<TParam, TResponse> request, TParam qrit, String source = null)
			where TParam : IQriteria;
	}
}