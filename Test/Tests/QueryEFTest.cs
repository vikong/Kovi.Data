﻿using System;
using System.Collections.Generic;
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
	public class EFModule : Autofac.Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder.RegisterType<AutofacInstanceFactory>()
				.As<IInstanceFactory>()
				.SingleInstance();

			// сервис
			builder.RegisterGeneric(typeof(LinqQueryHandler<,>))
				.As(typeof(IQueryHandler<,>));

			// контекст
			builder.RegisterType<BookContextFactory>()
				.As<IDbContextFactory>();

			builder.RegisterType<EFLinqProviderFactory>()
				.As<ILinqProviderFactory>()
				.SingleInstance();


			// запросы
			builder.RegisterType<SimpleQuery>()
				.As<IQueryHandler<StringQriteria, QueryResult<String>>>();

			builder.RegisterType<AllAuthorLinqQuery>()
				 .As<ILinqQuery<LinqQriteria, IEnumerable<Author>>>();

			builder.RegisterType<AuthorLinqQuery>()
				 .As<ILinqQuery<NameQriteria, IEnumerable<Author>>>();

		}
	}

	[TestClass]
	public class QueryEFTest
		: BaseAutofacTest
	{
		protected override Module AutofacModule => new EFModule();

		[TestMethod]
		public void SimpleQuery_ReturnsData()
		{
			var qh = Container.Resolve<IQueryHandler<StringQriteria, QueryResult<IEnumerable<String>>>>();
			var qrit = new StringQriteria()
			{
				Name = "A",
			};

			var actual = qh.Handle(qrit);

			Assert.AreEqual(nameof(SimpleQuery), actual.Stack.FirstOrDefault());

		}

		[TestMethod]
		public void LinqProviderFactory_Creates_Context()
		{
			var linq = Container.Resolve<ILinqProviderFactory>().Create();
			
			Assert.IsInstanceOfType(linq, typeof(EFLinqProvider));
			Assert.IsTrue(linq.Query<Book>().Any());
		}

		[TestMethod]
		public void LinqQueryHandler_Returns_Data()
		{
			var qh = Container.Resolve<IQueryHandler<LinqQriteria, IEnumerable<Author>>>();
			var qrit = new LinqQriteria();
			
			var actual = qh.Handle(qrit);

			Assert.IsTrue(actual.Any());
		}

		[TestMethod]
		public void LinqQueryHandler_Returns_FilteredData()
		{
			var qh = Container.Resolve<IQueryHandler<NameQriteria, IEnumerable<Author>>>();
			var qrit = new NameQriteria() { Name="T" };

			var actual = qh.Handle(qrit);

			Assert.IsTrue(actual.All(a=>a.Name.StartsWith(qrit.Name)));
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

		//		ILinqQueryHandlerOld<NameQriteria, IEnumerable<Author>> handler =
		//			new LinqQueryHandlerOld<NameQriteria, IEnumerable<Author>>(linqProviderFactoryStub);

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