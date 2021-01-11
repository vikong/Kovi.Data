using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using Kovi.Data.Cqrs;
using Kovi.Data.Cqrs.Linq;

using Moq;

namespace Data.Cqrs.Test
{
	internal class FakeDbSet<T> : Mock<DbSet<T>> 
		where T : class
	{
		public void SetData(IEnumerable<T> data)
		{
			var mockDataQueryable = data.AsQueryable();

			As<IQueryable<T>>().Setup(x => x.Provider).Returns(mockDataQueryable.Provider);
			As<IQueryable<T>>().Setup(x => x.Expression).Returns(mockDataQueryable.Expression);
			As<IQueryable<T>>().Setup(x => x.ElementType).Returns(mockDataQueryable.ElementType);
			As<IQueryable<T>>().Setup(x => x.GetEnumerator()).Returns(mockDataQueryable.GetEnumerator());
		}
	}

	internal static class Stub
	{

		public static IUnitOfWork CreateMemoryUoW(params Type[] types)
		{
			var uow = new InMemoryStore();
			foreach (var t in types)
				uow.Register(t);
			return uow;
		}

		public static IUnitOfWork Add<TEntity>(this IUnitOfWork uow, int numOfEntities, Func<Int32,TEntity> generator)
			where TEntity : class, IHasId<Int32>
		{
			var entities = uow.Linq.Query<TEntity>();
			int startIdx = entities.Any() ? entities.Max((a) => a.Id) + 1 : 1;
			Parallel.For(startIdx, startIdx + numOfEntities, i =>
			{
				uow.Add(generator(i));
			});
			return uow;
		}

		public static IUnitOfWork AddAuthors(this IUnitOfWork uow, int authorCount, string authorPred = "Author #")
		{
			var query = uow.Linq.Query<Author>();
			int idx = query.Any() ? query.Max((a) => a.Id) + 1 : 1;
			Parallel.For(idx, idx + authorCount, i =>
			{
				uow.Add(new Author(i, authorPred + Convert.ToString(i).PadLeft(3, '0')));
			});
			return uow;
		}

		public static ILinqProviderFactory LinqProviderFactory(this IUnitOfWork uow)
		{
			var linqProviderFactoryStub = new Mock<ILinqProviderFactory>();
			linqProviderFactoryStub.Setup(a => a.Create(It.IsAny<String>(), It.IsAny<String>())).Returns(uow.Linq);
			return linqProviderFactoryStub.Object;

		}
	}

}
