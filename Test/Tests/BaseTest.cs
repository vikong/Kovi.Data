using Autofac;

namespace Data.Cqrs.Test
{
	public abstract class BaseAutofacTest
	{
		protected static IContainer Container { get; private set; }
		//protected static IServiceLocator Sl { get; private set; }

		public BaseAutofacTest()
		{
			var builder = new ContainerBuilder();

			builder.RegisterModule(AutofacModule);

			Container = builder.Build();

			//Sl = new AutofacServiceLocator(Container);
		}

		protected abstract Module AutofacModule { get; }
	}
}