using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kovi.Data.Cqrs.Linq
{
	public class BaseLinquery<TParam, TResponse>
		: RequestQuery<TParam, TResponse>
		where TParam : IQriteria
	{
		protected BaseLinquery(ILinqRequestHandler linqRequestHandler, ILinqConvertor<TResponse> convertor) 
			: base(linqRequestHandler)
		{
			Convertor = convertor;
		}

		public BaseLinquery(ILinqSpec<TParam> linqSpec, ILinqRequestHandler linqRequestHandler, ILinqConvertor<TResponse> convertor)
			:this(linqRequestHandler, convertor)
		{
			Spec = linqSpec;
		}

		private ILinqConvertor<TResponse> Convertor { get; }

		public virtual ILinqSpec<TParam> Spec { get; }

		public override TResponse Request(ILinqProvider linqProvider, TParam qrit)
			=> linqProvider.Apply(Spec, qrit).Convert(Convertor, qrit);

		public override Task<TResponse> RequestAsync(ILinqProvider linqProvider, TParam qrit)
			=> linqProvider.Apply(Spec, qrit).ConvertAsync(Convertor, qrit);
	}

	public abstract class Linquery<TParam, TResponse>
		: BaseLinquery<TParam, TResponse>
		, ILinqSpec<TParam>
		where TParam : IQriteria
	{
		protected Linquery(ILinqRequestHandler linqRequestHandler, ILinqConvertor<TResponse> convertor)
			:base(linqRequestHandler,convertor)
		{
		}

		public override sealed ILinqSpec<TParam> Spec => this;

		public abstract IQueryable Query(ILinqProvider linqProvider, TParam qrit);
	}

	public class SpecLinqQuery<TParam, TEntity, TResponse>
		: RequestQuery<TParam, TResponse>
		where TParam : IQriteria
		where TEntity : class
	{
		protected SpecLinqQuery(ILinqRequestHandler linqRequestHandler, ILinqConvertor<TResponse> convertor)
			: base(linqRequestHandler)
		{
			Convertor = convertor;
		}

		public SpecLinqQuery(ILinqSpec<TParam, TEntity> linqSpec, ILinqRequestHandler linqRequestHandler, ILinqConvertor<TResponse> convertor)
			: this(linqRequestHandler, convertor)
		{
			Spec = linqSpec;
		}
		public SpecLinqQuery(IFilterSpec<TParam, TEntity> filterSpec, ILinqRequestHandler handler, ILinqConvertor<TResponse> linqConvertor)
			: this(LinqSpec<TEntity>.Filter(filterSpec), handler, linqConvertor)
		{ }


		private ILinqConvertor<TResponse> Convertor { get; }

		public virtual ILinqSpec<TParam, TEntity> Spec { get; }

		private IQueryable<TEntity> PreQuery(ILinqProvider linqProvider, TParam qrit)
		{
			var res = linqProvider.Query<TEntity>()
			.Apply(Spec, qrit);
			return res;
		}

		public override sealed TResponse Request(ILinqProvider linqProvider, TParam qrit)
			=> PreQuery(linqProvider, qrit).Convert(Convertor, qrit);

		public override sealed Task<TResponse> RequestAsync(ILinqProvider linqProvider, TParam qrit)
			=> PreQuery(linqProvider, qrit).ConvertAsync(Convertor, qrit);
	}

	public abstract class Linquery<TParam, TEntity, TResponse>
		: SpecLinqQuery<TParam, TEntity, TResponse>
		, ILinqSpec<TParam, TEntity>
		where TParam : IQriteria
		where TEntity : class
	{
		protected Linquery(ILinqRequestHandler linqRequestHandler, ILinqConvertor<TResponse> convertor)
			: base(linqRequestHandler, convertor)
		{}

		public override sealed ILinqSpec<TParam, TEntity> Spec => this;

		public abstract IQueryable<TEntity> Query(IQueryable<TEntity> query, TParam qrit);
	}
}