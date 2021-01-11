using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kovi.Data.Cqrs.Linq
{
	public class ConvertorLinqRequest<TParam, TResponse>
		: ILinqRequest<TParam, TResponse>
		where TParam : IQriteria
	{
		public ConvertorLinqRequest(ILinqConvertor<TResponse> convertor)
		{
			Convertor = convertor;
		}
		protected ILinqConvertor<TResponse> Convertor { get; }

		protected ILinqSpec<TParam> LinqSpec { get; }

		public TResponse Request(ILinqProvider linqProvider, TParam qrit)
		=> linqProvider.Apply(LinqSpec, qrit)
			.Convert(Convertor, qrit);

		public Task<TResponse> RequestAsync(ILinqProvider linqProvider, TParam qrit)
		=> linqProvider.Apply(LinqSpec, qrit)
			.ConvertAsync(Convertor, qrit);
	}
}
