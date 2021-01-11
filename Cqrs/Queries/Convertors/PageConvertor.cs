using System;
using System.Linq;
using System.Threading.Tasks;

using Kovi.LinqExtensions;

namespace Kovi.Data.Cqrs.Linq
{
	/// <summary>
	/// Обеспечивает конвертацию IQueryable в IPage{TResponse}
	/// </summary>
	/// <typeparam name="TResponse">объект данных Dto</typeparam>
	public class PageConvertor<TResponse>
		: BaseLinqConvertor<IPage<TResponse>>
		, IPageConvertor<TResponse>
		//where TResponse : class
	{

		public PageConvertor(IProjector projector)
			: base(projector) { }

		public override IPage<TResponse> Convert(IQueryable query, Object param)
		{
			//if (!typeof(IQueryable<TEntity>).IsAssignableFrom(query.Expression.Type))
			//	throw new ArgumentException("Query must be typed for pagination.", nameof(query));
			if (!typeof(IPaging).IsAssignableFrom(param.GetType()))
				throw new ArgumentException("Parameter must release IPaging interface for pagination.", nameof(param));

			//return Projector.ProjectToPage<TEntity,TResponse>((IQueryable<TEntity>)query, param as IPaging);
			return Projector.ProjectToPage<TResponse>(query, param as IPaging);

		}

		public override Task<IPage<TResponse>> ConvertAsync(IQueryable query, Object param)
		{
			//if (!typeof(IQueryable<TEntity>).IsAssignableFrom(query.Expression.Type))
			//	throw new ArgumentException("Query must be typed for pagination.", nameof(query));
			if (!typeof(IPaging).IsAssignableFrom(param.GetType()))
				throw new ArgumentException("Parameter must release IPaging interface for pagination.", nameof(param));

			return Projector.ProjectToPageAsync<TResponse>(query, param as IPaging);
		}
	}


}
