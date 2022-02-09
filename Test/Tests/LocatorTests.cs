using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autofac;
using Data.Cqrs.Test.EF;
using Kovi.Autofac;
using Kovi.Data.Cqrs;
using Kovi.Data.Cqrs.Infrastructure;
using Kovi.Data.Cqrs.Linq;
using Kovi.Data.EF;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Data.Cqrs.Test
{
	public class ServiceModule : Autofac.Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder.RegisterType<AutofacInstanceFactory>()
				.As<IInstanceFactory>()
				.SingleInstance();


			// сервис
			builder.RegisterType<HandlerLocator>()
				.As<ICqHandlerService>()
				.SingleInstance();

			builder.RegisterGenericDecorator(typeof(TestDecoratorQueryHandler<,>), typeof(IQueryHandler<,>));

			builder.RegisterGeneric(typeof(LinqQueryHandlerStub<,>))
				.As(typeof(IQueryHandler<,>));

			// запросы
			builder.RegisterType<SimpleLinqQuery>()
				.As<ILinqQuery<SimpleLinqQriteria, QueryResult<String>>>();

			builder.RegisterType<SimpleQuery>()
				.As<IQueryHandler<SimpleQriteria, QueryResult<String>>>();

			builder.RegisterType<SimpleIdQuery>()
				.As<IQueryHandler<IdQriteria, QueryResult<String>>>();

			// команды
			builder.RegisterType<SimpleCommandHandler>()
				.As<ICommandHandler<SimpleCommand>>();
		}
	}

	[TestClass]
	public class LocatorTests
		: BaseAutofacTest
	{
		protected override Module AutofacModule => new ServiceModule();

		[TestMethod]
		public void ServiceLocator_Resolve_ReturnsQueryHandler()
		{
			Object actual;
			
			actual = Container.Resolve<IQueryHandler<SimpleQriteria, QueryResult<String>>>();
			Assert.IsInstanceOfType(actual, typeof(TestDecoratorQueryHandler<SimpleQriteria, QueryResult<String>>));

			actual = Container.Resolve<IQueryHandler<SimpleLinqQriteria, QueryResult<String>>>();
			Assert.IsInstanceOfType(actual, typeof(TestDecoratorQueryHandler<SimpleLinqQriteria, QueryResult<String>>));
		}


		[TestMethod]
		public void ServiceLocator_Resolve_ReturnsLinqQueryHandler()
		{
			var handler = Container.Resolve<IQueryHandler<SimpleLinqQriteria, QueryResult<String>>>();

			var qrit = new SimpleLinqQriteria { Name = "SimpleLinqQriteria" };

			var actual = handler.Handle(qrit);

			foreach (var item in actual.Stack)
			{
				Debug.WriteLine(item);
			}

			Assert.AreEqual($"{SimpleLinqQuery.Data}:{qrit.Name}", actual.Data);
			Assert.AreEqual(actual.Stack[0], SimpleLinqQuery.Data);
			//Assert.AreEqual(actual.Stack[1], "TestDecoratorQueryHandler");
			//Assert.IsInstanceOfType(actual, typeof(TestDecoratorQueryHandler<SimpleLinqQriteria, QueryResult<String>>));
		}

		[TestMethod]
		public void HandlerLocator_ForQriteria_ReturnsQuery()
		{
			var qrit = new SimpleQriteria { Name = "А" };

			var sl = Container.Resolve<ICqHandlerService>();

			var actual = sl
				.For<QueryResult<String>>()
				.Ask(qrit);

			Assert.AreEqual($"SimpleQuery:{qrit.Name}", actual.Data);
			Assert.AreEqual(actual.Stack[0], "SimpleQuery");
			Assert.AreEqual(actual.Stack[1], "TestDecoratorQueryHandler");

		}

		[TestMethod]
		public void HandlerLocator_ForCommand_ReturnsCommandHandler()
		{
			var cmd = new SimpleCommand { Name = "А" };

			var sl = Container.Resolve<ICqHandlerService>();

			var actual = sl.Process(cmd);


			Assert.IsTrue(actual.IsSuccess);
			var ret = actual.Return(r => ((List<String>)r), f => throw new Exception());

			Assert.AreEqual($"{SimpleCommandHandler.Data}:{cmd.Name}", ret[0]);

		}

		//[TestMethod]
		//public void TestMethod1()
		//{
		//	//Arrange
		//	const int authorCount = 25;

		//	using (var UoWStub = Stub.CreateMemoryUoW(typeof(Author))
		//		.Add(
		//			authorCount,
		//			(i) => new Author(i, "A" + (i % 2 == 0 ? "_" : "-") + Convert.ToString(i).PadLeft(2, '0'))
		//		))
		//	{
		//		ILinqProviderFactory linqProviderFactoryStub = UoWStub.LinqProviderFactory();

		//		ILinqQueryHandler<NameQriteria, IEnumerable<Author>> handler =
		//			new LinqQueryHandler<NameQriteria, IEnumerable<Author>>(linqProviderFactoryStub);

		//		var qrit = new NameQriteria { Name = "2" };

		//		var query = new AuthorLinqQuery(handler);

		//		var expected = linqProviderFactoryStub.Create()
		//			.Query<Author>()
		//			.Where(a => a.Name.Contains(qrit.Name))
		//			.ToArray();

		//		//Act
		//		var actual = query.Handle(qrit);

		//		//Assert
		//		CollectionAssert.AreEqual(expected, actual.ToArray());
		//	}
		//}
	}
}