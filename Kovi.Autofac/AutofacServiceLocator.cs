using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;
using Kovi.Data.Cqrs;

namespace Kovi.Autofac
{
	public class AutofacServiceLocator 
		: IServiceLocator
	{
		private readonly IContainer container;

		public AutofacServiceLocator(IContainer autofacContainer)
		{
			container = autofacContainer;
		}

		public TService GetInstance<TService>() => container.Resolve<TService>();

		public Object GetInstance(Type service) => container.Resolve(service);

		public T TryGetInstance<T>()
			where T: class
		{
			if (container.TryResolve<T>(out T result))
				return result;
			else
				return default(T);
		}

		public Object TryGetInstance(Type service)
		{
			if (container.TryResolve(service, out Object result))
				return result;
			else
			if (service.IsValueType)
				return Activator.CreateInstance(service);

			return null;
		}
	}
}
