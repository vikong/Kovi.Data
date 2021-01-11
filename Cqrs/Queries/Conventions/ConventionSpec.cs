using System.Linq;

using Kovi.Data.Cqrs.Infrastructure;
using Kovi.LinqExtensions;

namespace Kovi.Data.Cqrs.Linq
{
	public class ConventionIdLinqSpec<TEntity>
		: ILinqSpec<IHasId>
		where TEntity : class, IHasId
	{
		public IQueryable Query(ILinqProvider linqProvider, IHasId qrit)
			=> linqProvider.Query<TEntity>()
			.Where(ConventionBuilder<TEntity>.IdFilterExpression(qrit));
	}

	public class ConventionFilterSpec<TEntity, TParam>
		: ILinqSpec<TParam, TEntity>
		where TEntity : class
	{
		public IQueryable<TEntity> Query(IQueryable<TEntity> query, TParam qrit)
			=> query.Where(ConventionBuilder<TEntity>.FilterExpression(qrit));
	}

	public sealed class ConventionPagedSpec<TEntity, TParam>
		: ILinqSpec<IPageQriteria<TParam>>
		where TEntity : class
	{
		public IQueryable Query(ILinqProvider linqProvider, IPageQriteria<TParam> qrit)
		{
			var filterExpr = qrit.Subject != null ?
				ConventionBuilder<TEntity>.FilterExpression(qrit.Subject)
				: (x) => true;

			var query = linqProvider
				.Query<TEntity>()
				.Where(filterExpr)
				.OrderByConventions(qrit);

			return query;
		}
	}

}