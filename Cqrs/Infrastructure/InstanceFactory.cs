using System;

namespace Kovi.Data.Cqrs.Infrastructure
{
	public delegate Object InstanceFactory(Type serviceType);

	public interface IInstanceFactory
	{
		Object Resolve(Type t);
	}
}