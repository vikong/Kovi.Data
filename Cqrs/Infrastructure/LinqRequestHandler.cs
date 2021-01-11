using System;
using System.Threading.Tasks;

namespace Kovi.Data.Cqrs.Linq
{
	public class LinqRequestHandler
		: ILinqRequestHandler
	{
		private ILinqProviderFactory LinqProviderFactory { get; }

		public LinqRequestHandler(ILinqProviderFactory linqProviderFactory)
		{
			LinqProviderFactory = linqProviderFactory ?? throw new ArgumentNullException(nameof(linqProviderFactory));
		}

		public TResponse Perform<TParam, TResponse>(ILinqRequest<TParam, TResponse> request, TParam qrit, String source = null)
			where TParam : IQriteria
		{
			using (var linqProvider = LinqProviderFactory.Create(source))
			{
				return request.Request(linqProvider, qrit);
			}
		}

		public async Task<TResponse> PerformAsync<TParam, TResponse>(ILinqRequest<TParam, TResponse> request, TParam qrit, String source = null)
			where TParam : IQriteria
		{
			using (var linqProvider = LinqProviderFactory.Create(source))
			{
				return await request.RequestAsync(linqProvider, qrit);
			}
		}
	}
}