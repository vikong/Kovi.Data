using System;
using System.Linq;
using System.Threading.Tasks;

using Kovi.LinqExtensions;

namespace Kovi.Data.Cqrs.Linq
{

	//public abstract class Linqur<TParam, TResponse>
	//	: RequestQuery<TParam, TResponse>
	//	where TParam: IQriteria
	//{
	//	protected Linqur(ILinqRequestHandler requestHandler, ILinqConvertor<TResponse> linqConvertor)
	//		:base(requestHandler)
	//	{
	//		Convertor = linqConvertor ?? throw new ArgumentNullException(nameof(linqConvertor));
	//	}

	//	private ILinqConvertor<TResponse> Convertor { get; }

	//	protected abstract ILinqSpec<TParam> Spec { get; }

	//	public override TResponse Request(ILinqProvider linqProvider, TParam qrit)
	//		=> Convertor.Convert(linqProvider, Spec, qrit);

	//	public override Task<TResponse> RequestAsync(ILinqProvider linqProvider, TParam qrit)
	//		=> Convertor.ConvertAsync(linqProvider, Spec, qrit);

	//}
	//public class Linqur<TParam, TEntity, TResponse>
	//: RequestQuery<TParam, TResponse>
	//, ILinqRequest<TParam, TResponse>
	//	where TParam : class, IQriteria
	//	where TEntity: class
	//{
	//	protected Linqur(ILinqRequestHandler requestHandler, ILinqConvertor<TResponse> linqConvertor)
	//	: base(requestHandler)
	//	{
	//		Convertor = linqConvertor ?? throw new ArgumentNullException(nameof(linqConvertor));
	//	}

	//	private ILinqConvertor<TResponse> Convertor { get; }

	//	protected ILinqSpec<TParam,TEntity> Spec { get; }

	//	public override TResponse Request(ILinqProvider linqProvider, TParam qrit)
	//		=> Convertor.Convert(linqProvider, Spec, qrit);

	//	//public override Task<TResponse> RequestAsync(ILinqProvider linqProvider, TParam qrit)
	//	//	=> linqProvider.Apply(this, qrit)
	//	//	.ConvertAsync(Convertor, qrit);

	//}


	//public abstract class Linquery<TParam, TResponse>
	//	: RequestQuery<TParam, TResponse>
	//	, ILinqSpec<TParam>
	//	where TParam : class, IQriteria
	//{
	//	protected Linquery(ILinqRequestHandler requestHandler, ILinqConvertor<TResponse> linqConvertor)
	//		:base(requestHandler)
	//	{
	//		Convertor = linqConvertor ?? throw new ArgumentNullException(nameof(linqConvertor));
	//	}

	//	private ILinqConvertor<TResponse> Convertor { get; }

	//	public override TResponse Request(ILinqProvider linqProvider, TParam qrit)
	//		=> linqProvider.Apply(this, qrit)
	//		.Convert(Convertor, qrit);

	//	public override Task<TResponse> RequestAsync(ILinqProvider linqProvider, TParam qrit)
	//		=> linqProvider.Apply(this, qrit)
	//		.ConvertAsync(Convertor, qrit);

	//	public abstract IQueryable Query(ILinqProvider linqProvider, TParam qrit);
	//}

	//public abstract class Linquery<TParam, TEntity, TResponse>
	//	: RequestQuery<TParam, TResponse>
	//	, ILinqSpec<TParam, TEntity>
	//	where TParam : class, IQriteria
	//	where TEntity : class
	//{
	//	protected Linquery(ILinqRequestHandler requestHandler, ILinqConvertor<TResponse> linqConvertor)
	//		:base(requestHandler)
	//	{
	//		Convertor = linqConvertor ?? throw new ArgumentNullException(nameof(linqConvertor));
	//	}

	//	private ILinqConvertor<TResponse> Convertor { get; }

	//	private IQueryable<TEntity> PreQuery(ILinqProvider linqProvider, TParam qrit)
	//	{
	//		var res = linqProvider.Query<TEntity>()
	//		.Apply(this, qrit);
	//		return res;
	//	}

	//	public override TResponse Request(ILinqProvider linqProvider, TParam qrit)
	//		=> PreQuery(linqProvider, qrit).Convert(Convertor, qrit);
			
	//	public override Task<TResponse> RequestAsync(ILinqProvider linqProvider, TParam qrit)
	//		=> PreQuery(linqProvider, qrit).ConvertAsync(Convertor, qrit);

	//	public abstract IQueryable<TEntity> Query(IQueryable<TEntity> query, TParam qrit);
	//}
}