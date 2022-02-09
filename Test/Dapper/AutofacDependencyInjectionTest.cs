using System;

using Autofac;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using Kovi.Data.Cqrs;
using Kovi.Data.Cqrs.Linq;
using Data.Cqrs.Test.EF;
using Kovi.Data.EF;
using System.Linq;
using Kovi.Autofac;
using Kovi.Data.Cqrs.Infrastructure;

namespace Data.Cqrs.Test
{
	public class DependencyModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<AutofacInstanceFactory>();
            builder.Register<InstanceFactory>(c =>
            {
                var sf = c.Resolve<IComponentContext>().Resolve<AutofacInstanceFactory>();
                return sf.Resolve;
            });

            // контекст
            builder.RegisterType<BookContextFactory>()
                .As<IDbContextFactory>();

            builder.RegisterType<EFLinqProviderFactory>()
                .As<ILinqProviderFactory>();

            builder.RegisterGenericDecorator(typeof(TestDecoratorQueryHandler<,>), typeof(IQueryHandler<,>));

            // запросы
            builder.RegisterType<SimpleQuery>()
                .As<IQueryHandler<SimpleQriteria, QueryResult<IEnumerable<String>>>>();

            builder.RegisterType<SimpleIdQuery>()
                .As<IQueryHandler<IdQriteria, QueryResult<String>>>();

            builder.RegisterType<AllAuthorLinqQuery>()
                .As<IQueryHandler<LinqQriteria, IEnumerable<Author>>>();
        }
    }

    [TestClass]
    public class AutofacDependencyInjectionTest
        : BaseAutofacTest
    {

        protected override Module AutofacModule 
            => new DependencyModule();

        [TestMethod]
        public void TestMethod1()
        {
            var qh = Container.Resolve<IQueryHandler<SimpleQriteria, QueryResult<IEnumerable<String>>>>();
            var qrit = new SimpleQriteria()
            {
                Name = "A",
            };

            var actual = qh.Handle(qrit);

            Assert.AreEqual(nameof(SimpleQuery), actual.Stack.FirstOrDefault());
        }


        [TestMethod]
        public void TestMethod3()
        {
            var handler = Container.Resolve<IQueryHandler<Qrit2, QResult>>();
            Debug.WriteLine(handler.GetType().Name);
            var q = new Qrit2() { Id = 1 };
            var actual = handler.Handle(q);
            Debug.WriteLine(actual);
            //Assert.IsInstanceOfType(q, typeof(Query1));
            Assert.IsNotNull(handler);
        }
    }
}
