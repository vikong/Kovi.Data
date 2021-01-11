using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kovi.Data.Cqrs.Linq
{
	public abstract class BaseLinqConvertor<TResponse>
		: ILinqConvertor<TResponse>
		where TResponse : class
	{
		protected IProjector Projector { get; }

		protected BaseLinqConvertor(IProjector projector)
		{
			Projector = projector ?? throw new ArgumentNullException(nameof(projector));
		}

		public abstract TResponse Convert(IQueryable query, Object param = null);

		public abstract Task<TResponse> ConvertAsync(IQueryable query, Object param=null);
	}

	public class SingleLinqConvertor<TResponse>
		: BaseLinqConvertor<TResponse>
		, ISingleLinqConvertor<TResponse>
		where TResponse : class
	{

		public SingleLinqConvertor(IProjector projector)
			: base(projector)
		{ }

		public override TResponse Convert(IQueryable query, Object param = null)
			=> query.ProjectToSingle<TResponse>(Projector);

		public override Task<TResponse> ConvertAsync(IQueryable query, Object param=null)
			=> query.ProjectToSingleAsync<TResponse>(Projector);
	}

	public class EnumLinqConvertor<TResponse>
		: BaseLinqConvertor<IEnumerable<TResponse>>
		, IEnumLinqConvertor<TResponse>
		where TResponse : class
	{
		public EnumLinqConvertor(IProjector projector) 
			: base(projector) 
		{ }

		public override IEnumerable<TResponse> Convert(IQueryable query, Object param = null)
			=> query.ProjectToEnum<TResponse>(Projector);

		public override Task<IEnumerable<TResponse>> ConvertAsync(IQueryable query, Object param=null)
			=> query.ProjectToEnumAsync<TResponse>(Projector);
	}

}
