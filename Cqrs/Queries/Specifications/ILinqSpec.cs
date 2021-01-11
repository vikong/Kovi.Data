using System.Linq;

namespace Kovi.Data.Cqrs.Linq
{
	public interface ILinqSpec<TParam>
	{
		IQueryable Query(ILinqProvider linqProvider, TParam qrit);
	}

	public interface ILinqSpec<TParam, TEntity>
		where TEntity : class
	{
		IQueryable<TEntity> Query(IQueryable<TEntity> query, TParam qrit);
	}
}