using Autofac;
using Kovi.Autofac;
using Kovi.Data.Cqrs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Cqrs.Test
{
    public abstract class BaseAutofacTest
    {
        protected static IContainer Container { get; private set; }
        protected static IServiceLocator Sl { get; private set; }

        public BaseAutofacTest()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule(AutofacModule);

            Container = builder.Build();

            Sl = new AutofacServiceLocator(Container);

        }

        protected abstract Module AutofacModule { get; }
    }
}
