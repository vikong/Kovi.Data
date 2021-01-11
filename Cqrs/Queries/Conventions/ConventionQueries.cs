using System;
using System.Threading.Tasks;

using Kovi.LinqExtensions;

namespace Kovi.Data.Cqrs.Linq
{
	public class ConventionIdQuery<TResponse>
		: IByIdQuery<TResponse>
		where TResponse : class
	{
		private readonly BaseLinquery<IHasId, TResponse> query;

		public ConventionIdQuery(ILinqRequestHandler handler, ISingleLinqConvertor<TResponse> linqConvertor)
		{
			query = new BaseLinquery<IHasId, TResponse>(Conventions<TResponse>.IdSpec, handler, linqConvertor);
		}

		public TResponse Ask(IHasId qrit, String source = null)
			=> ((IQuery<IHasId, TResponse>)query).Ask(qrit, source);

		public Task<TResponse> AskAsync(IHasId qrit, String source = null)
			=> ((IQuery<IHasId, TResponse>)query).AskAsync(qrit, source);
	}

	public class ConventionPagedQuery<TParam, TResponse>
		: IQuery<IPageQriteria<TParam>, IPage<TResponse>>
		where TResponse : class
	{
		private readonly BaseLinquery<IPageQriteria<TParam>, IPage<TResponse>> Query;

		public ConventionPagedQuery(ILinqRequestHandler handler, IPageConvertor<TResponse> convertor)
		{
			Query = new BaseLinquery<IPageQriteria<TParam>, IPage<TResponse>>(
				Conventions<TResponse>.PageSpec<TParam>(),
				handler,
				convertor
			);
		}

		public IPage<TResponse> Ask(IPageQriteria<TParam> qrit, String source = null)
			=> ((IQuery<IPageQriteria<TParam>, IPage<TResponse>>)Query).Ask(qrit, source);

		public Task<IPage<TResponse>> AskAsync(IPageQriteria<TParam> qrit, String source = null)
			=> ((IQuery<IPageQriteria<TParam>, IPage<TResponse>>)Query).AskAsync(qrit, source);
	}
}