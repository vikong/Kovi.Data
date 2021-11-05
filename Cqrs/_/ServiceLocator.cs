using System;
using System.Collections.Generic;
using System.Linq;
using Kovi.Data.Cqrs.Infrastructure;

namespace Kovi.Data.Cqrs
{
	/// <summary>
	///  делегат разрешения зависимостей для сервис-локатора
	/// </summary>
	/// <param name="serviceType">тип</param>
	/// <returns></returns>
	//public delegate Object InstanceFactory(Type serviceType);

	//public delegate IEnumerable<Object> MultiInstanceFactory(Type serviceType);

	public class ServiceLocator : IServiceLocator
	{
		protected InstanceFactory SingleInstanceFactory { get; }

		//protected MultiInstanceFactory MultiInstanceFactory { get; }

		public ServiceLocator(InstanceFactory instanceFactory)//, MultiInstanceFactory multiInstanceFactory)
		{
			SingleInstanceFactory = instanceFactory;
			//MultiInstanceFactory = multiInstanceFactory;
		}

		public T GetInstance<T>()
			=> (T)GetInstance(typeof(T));

		public T GetInstance<T>(Type t)
			=> (T)GetInstance(t);

		public Object GetInstance(Type t)
		{
			try
			{
				return SingleInstanceFactory(t);
			}
			catch (Exception e)
			{
				throw BuildException(t, e);
			}
		}

		//public IEnumerable<Object> GetAllInstances(Type t)
		//	=> MultiInstanceFactory(t);

		//public IEnumerable<T> GetAllInstances<T>()
		//	=> GetAllInstances(typeof(T)).Cast<T>();

		public Object TryGetInstance(Type t)
		{
			try
			{
				return SingleInstanceFactory(t);
			}
			catch { }

			return null;
		}

		public T TryGetInstance<T>() 
			where T : class
		{
			return (T)TryGetInstance(typeof(T));
		}

		private static InvalidOperationException BuildException(Type requestType, Exception inner)
			=> new InvalidOperationException($"Error while initializing service of type {requestType} .\r\nContainer or service locator not configured properly or handlers not registered with your container.", inner);
	}
}