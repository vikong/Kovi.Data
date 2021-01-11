using System;
using System.Threading.Tasks;

namespace Kovi.Data.Cqrs.Linq
{
	public class BaseRequestQuery<TParam, TResponse>
		: IQuery<TParam, TResponse>
		where TParam : IQriteria
	{
		protected BaseRequestQuery(ILinqRequestHandler requestHandler)
		{
			Handler = requestHandler ?? throw new ArgumentNullException(nameof(requestHandler));
		}

		protected BaseRequestQuery()
		{ }

		public BaseRequestQuery(ILinqRequest<TParam, TResponse> linqRequest, ILinqRequestHandler requestHandler)
			:this(requestHandler)
		{
			LinqRequest = linqRequest;
		}

		private ILinqRequestHandler Handler { get; }

		TResponse IQuery<TParam, TResponse>.Ask(TParam qrit, String source)
			=> Handler.Perform(LinqRequest, qrit, source);

		Task<TResponse> IQuery<TParam, TResponse>.AskAsync(TParam qrit, String source)
			=> Handler.PerformAsync(LinqRequest, qrit, source);

		public virtual ILinqRequest<TParam, TResponse> LinqRequest { get; }

	}

	public abstract class RequestQuery<TParam, TResponse>
		: BaseRequestQuery<TParam, TResponse>
		, ILinqRequest<TParam, TResponse>
		where TParam : IQriteria
	{
		protected RequestQuery(ILinqRequestHandler linqRequestHandler)
			:base(linqRequestHandler)
		{ }

		[Obsolete("Only for testing purposes", true)]
		protected RequestQuery()
		{ }

		public override sealed ILinqRequest<TParam, TResponse> LinqRequest => this;

		public abstract TResponse Request(ILinqProvider linqProvider, TParam qrit);

		public virtual Task<TResponse> RequestAsync(ILinqProvider linqProvider, TParam qrit)
			=> Task<TResponse>.Run(() => Request(linqProvider, qrit)); 
	}
}