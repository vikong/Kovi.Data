using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Data.Cqrs.Test.EF;

using Kovi.Data.Automapper;
using Kovi.Data.Cqrs;
using Kovi.Data.Cqrs.Infrastructure;
using Kovi.Data.Cqrs.Linq;
using Kovi.Data.EF;
using Kovi.LinqExtensions;

using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Modules;

namespace Data.Cqrs.Test
{
	public class FactoriesModule : NinjectModule
	{
		public override void Load()
		{
			Bind<IUnitOfWorkFactory>()
				.To<EFUnitOfWorkFactory>();

			Bind<ILinqProviderFactory>()
				.To<EFLinqProviderFactory>();

			Bind<IDbContextFactory>()
				.To<BookContextFactory>();

			Bind<IConnectionFactory>()
				.To<Dapper.BooksConnectionFactory>();
		}
	}

	public class QueriesModule : NinjectModule
	{
		public override void Load()
		{

			Bind<ILinqCommandHandler>()
				.To<LinqCommandHandler>();

			Bind<ILinqRequestHandler>()
				.To<LinqRequestHandler>();

			Bind<IProjector>()
				.To<AutoMapperProjector>();

			Bind(typeof(ISingleLinqConvertor<>))
				.To(typeof(SingleLinqConvertor<>));

			Bind(typeof(IEnumLinqConvertor<>))
				.To(typeof(EnumLinqConvertor<>));

			//Bind(typeof(INativeSingleLinqConvertor<>))
			//	.To(typeof(NativeLinqConvertor<>));

			//Bind(typeof(INativeEnumLinqConvertor<>))
			//	.To(typeof(NativeEnumLinqConvertor<>));

			//Bind(typeof(IDtoSingleLinqConvertor<>))
			//	.To(typeof(DtoSingleLinqConvertor<>));

			//Bind(typeof(IDtoEnumLinqConvertor<>))
			//	.To(typeof(DtoEnumLinqConvertor<>));

			//Bind(typeof(ISingleLinqConvertor<>))
			//	.To(typeof(DtoSingleLinqConvertor<>));

			//Bind(typeof(IEnumLinqConvertor<>))
			//	.To(typeof(EnumAutomapperConvertor<>));

			Bind(typeof(IPageConvertor<>))
				.To(typeof(PageConvertor<>));

			//Bind(typeof(IPageConvertor<>))
			//	.To(typeof(ConventionPageConvertor<>));


			//Bind(typeof(IQueryStub<,>))
			//	.To(typeof(IdQueryStub<,>));

			//*** Queries ***

			// Поиск по Id => IByIdQuery
			Bind(typeof(IByIdQuery<>))
				.To(typeof(ConventionIdQuery<>));

			// постраничный вывод по умолчанию для сущностей, помеченных атрибутом EntityMapAttribute
			var conventionalTypes = GetType().Assembly
				.GetTypes()
				.Where(t => t.GetTypeInfo().GetCustomAttribute<EntityMapAttribute>()?.ConventionQuery != null);

			foreach (Type t in conventionalTypes)
				BindPageQuery(t, typeof(ConventionPagedQuery<,>));

			// IQuery
			Kernel.Bind(x => 
				x.FromThisAssembly()
				.SelectAllClasses()
				.InheritedFrom(typeof(IQuery<,>))
				.BindAllInterfaces()
			);

			//Commands
			Kernel.Bind(x =>
				x.FromThisAssembly()
				.SelectAllClasses()
				.InheritedFrom(typeof(ICommand<>))
				.BindAllInterfaces()
			);
		}

		private void BindPageQuery(Type dtoType, Type pageQueryType)
		{
			Type qritType = typeof(IPageQriteria<>).MakeGenericType(dtoType);
			Type responseType = typeof(IPage<>).MakeGenericType(dtoType);

			Type queryType = typeof(IQuery<,>).MakeGenericType(qritType, responseType);

			Bind(queryType)
				.To(pageQueryType.MakeGenericType(dtoType, dtoType));
		}
	}

	public class NinjectServiceLocator : IServiceLocator
	{
		private readonly IKernel _kernel;

		public NinjectServiceLocator(IKernel kernelParam)
		{
			_kernel = kernelParam;
		}

		public IEnumerable<Object> GetAllInstances(Type t) 
			=> _kernel.GetAll(t);

		public IEnumerable<T> GetAllInstances<T>()
			=> _kernel.GetAll<T>();


		public T GetInstance<T>() 
			=> _kernel.Get<T>();

		public Object GetInstance(Type t) 
			=> _kernel.Get(t);


		public T TryGetInstance<T>() where T:class
			=> _kernel.TryGet<T>();

		public Object TryGetInstance(Type t) 
			=> _kernel.TryGet(t);
	}

}
