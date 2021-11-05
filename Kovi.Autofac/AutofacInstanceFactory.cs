using System;

using Autofac;
using Kovi.Data.Cqrs.Infrastructure;

namespace Kovi.Autofac
{
	public class AutofacInstanceFactory: IInstanceFactory
	{
		private readonly IComponentContext _componentContext;

		public AutofacInstanceFactory(IComponentContext componentContext)
		{
			_componentContext = componentContext;
		}

		public Object Resolve(Type t) => _componentContext.Resolve(t);
	}
}