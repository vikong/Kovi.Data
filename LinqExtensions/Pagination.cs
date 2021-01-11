using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// https://github.com/hightechgroup/costeffectivecode/tree/master/src/CostEffectiveCode/Ddd/Pagination
/// </summary>
namespace Kovi.LinqExtensions
{

	public interface IPaging
	{
		Int32 PageNo { get; }
		Int32 PageSize { get; }
	}

	public interface IPaging<TEntity> : IPaging
		where TEntity : class
	{
		IEnumerable< Sorting<TEntity, Object> > OrderBy { get; }
	}

	public interface IPage<out T> : IPaging//, IEnumerable<T>
	{
		/// <summary>
		/// Общее количество записей на всех страницах.
		/// </summary>
		Int64 TotalCount { get; }
		IEnumerable<T> Members { get; }
	}

	public static class PageExtensions
	{

		public static IQueryable<T> TakePage<T>(this IQueryable<T> queryable, IPaging paging) 
			=> queryable
				.Skip((paging.PageNo - 1) * paging.PageSize)
				.Take(paging.PageSize);

		public static IQueryable<T> TakePage<T>(this IQueryable<T> queryable, IPaging<T> paging)
			where T : class
		{
			if (!paging.OrderBy.Any())
			{
				throw new ArgumentException("OrderBy can't be null or empty", nameof(paging));
			}

			return queryable.OrderBy(paging.OrderBy)
				.Skip((paging.PageNo - 1) * paging.PageSize)
				.Take(paging.PageSize);
		}

		public static IPage<T> ToPage<T>(this IEnumerable<T> enumerable, Int64 totalCount, IPaging paging)
			=> new Page<T>(enumerable, totalCount, paging);

	}

	// добавить проверки на допустимость значений PageNo PageSize
	public class Paging : IPaging
	{
		public Int32 PageNo { get; set; }

		public Int32 PageSize { get; set; }

		public Paging(Int32 pageNo, Int32 pageSize)
		{
			PageNo = pageNo;
			PageSize = pageSize;
		}

		public Paging(IPaging paging)
		{
			PageNo = paging.PageNo;
			PageSize = paging.PageSize;
		}
	}

	public class Page<T> : IPage<T>
	{
		private readonly IEnumerable<T> _inner;

		public Int64 TotalCount { get; }

		public Int32 PageNo { get; }

		public Int32 PageSize { get; }

		public IEnumerable<T> Members => _inner;

		public Page(IEnumerable<T> inner, Int64 totalCount, Int32 pageNo, Int32 pageSize)
		{
			_inner = inner;
			TotalCount = totalCount;
			PageNo = pageNo;
			PageSize = pageSize;
		}


		public Page(IEnumerable<T> inner, Int64 totalCount, IPaging paging) 
			: this(inner, totalCount, paging.PageNo, paging.PageSize)
		{ }

		//public IEnumerator<T> GetEnumerator() 
		//	=> _inner.GetEnumerator();

		//IEnumerator IEnumerable.GetEnumerator() 
		//	=> GetEnumerator();

		public static Page<T> Empty(Int32 pageNo, Int32 pageSize)
			=> new Page<T>(Enumerable.Empty<T>(), 0, pageNo, pageSize);

		public static Page<T> Empty(IPaging paging)
			=> Empty(paging.PageNo, paging.PageSize);

		public static Page<T> Empty()
			=> Empty(0, 0);


	}
}
