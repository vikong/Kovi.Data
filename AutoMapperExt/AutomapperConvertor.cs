using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

namespace Kovi.Data.Cqrs.Linq.Automapper
{
	//public abstract class AutomapperConvertor<TDestination>
	//{
	//	protected readonly IMapper _mapper;

	//	public AutomapperConvertor(IMapper mapper)
	//	{
	//		_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
	//	}

	//	public abstract TDestination Convert(IQueryable query, Object param=null);

	//	public abstract Task<TDestination> ConvertAsync(IQueryable query, Object param=null);
	
	//}

	//public class SingleAutomapperConvertor<TDestination>
	//: AutomapperConvertor<TDestination>
	//, ISingleLinqConvertor<TDestination>
	//{
	//	public SingleAutomapperConvertor(IMapper mapper) : base(mapper)
	//	{ }

	//	public override TDestination Convert(IQueryable query, Object param=null)
	//		=> query.ProjectToSingleOrDefault<TDestination>(_mapper.ConfigurationProvider);

	//	public override Task<TDestination> ConvertAsync<TParam>(IQueryable query, TParam param)
	//		=> query.ProjectToSingleOrDefaultAsync<TDestination>(_mapper.ConfigurationProvider);
	//}


	//public class EnumAutomapperConvertor<TDestination>
	//	: AutomapperConvertor<IEnumerable<TDestination>>
	//	, IEnumLinqConvertor<TDestination>
	//{
	//	public EnumAutomapperConvertor(IMapper mapper) : base(mapper) 
	//	{ }

	//	public override IEnumerable<TDestination> Convert(IQueryable query, Object param=null)
	//		=> query.ProjectToList<TDestination>(_mapper.ConfigurationProvider);

	//	public override Task<IEnumerable<TDestination>> ConvertAsync<TParam>(IQueryable query, TParam param)
	//	{
	//		var res = query
	//			.ProjectToListAsync<TDestination>(_mapper.ConfigurationProvider)
	//			.ContinueWith(t=>t.Result.AsEnumerable<TDestination>());
	//		return res;
	//	}
	//}
}
