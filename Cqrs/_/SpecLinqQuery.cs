using System;
using System.Linq;

namespace Kovi.Data.Cqrs.Linq
{
	//public class SpecLinqQuery<TParam, TResponse>
	//	: Linquery<TParam, TResponse>
	//	, IQuery<TParam, TResponse>
	//	where TParam : class, IQriteria
	//{
	//	protected SpecLinqQuery(ILinqRequestHandler handler, ILinqConvertor<TResponse> linqConvertor)
	//		: base(handler, linqConvertor)
	//	{ }

	//	public SpecLinqQuery(ILinqSpec<TParam> linqSpec, ILinqRequestHandler handler, ILinqConvertor<TResponse> linqConvertor)
	//		: this(handler, linqConvertor)
	//	{
	//		Spec = linqSpec ?? throw new ArgumentNullException(nameof(linqSpec));
	//	}

	//	public override sealed IQueryable Query(ILinqProvider linqProvider, TParam qrit)
	//		=> linqProvider.Apply(Spec, qrit);

	//	public ILinqSpec<TParam> Spec { get; }
	//}

	//public class SpecLinqQuery<TParam, TEntity, TResponse>
	//	: Linquery<TParam, TEntity, TResponse>
	//	, IQuery<TParam, TResponse>
	//	where TParam : class, IQriteria
	//	where TEntity : class
	//{
	//	protected SpecLinqQuery(ILinqRequestHandler requestHandler, ILinqConvertor<TResponse> linqConvertor)
	//		: base(requestHandler, linqConvertor)
	//	{ }

	//	public SpecLinqQuery(ILinqSpec<TParam, TEntity> linqSpec, ILinqRequestHandler requestHandler, ILinqConvertor<TResponse> linqConvertor)
	//		: this(requestHandler, linqConvertor)
	//	{
	//		Spec = linqSpec ?? throw new ArgumentNullException(nameof(linqSpec));
	//	}

	//	public SpecLinqQuery(IFilterSpec<TParam, TEntity> filterSpec, ILinqRequestHandler handler, ILinqConvertor<TResponse> linqConvertor)
	//		: this(LinqSpec<TEntity>.Filter(filterSpec), handler, linqConvertor)
	//	{ }

	//	public override sealed IQueryable<TEntity> Query(IQueryable<TEntity> query, TParam qrit)
	//		=> query.Apply(Spec, qrit);

	//	public ILinqSpec<TParam, TEntity> Spec { get; }

	//}
}