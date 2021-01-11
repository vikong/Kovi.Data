using System;
using System.Collections.Generic;

namespace Kovi.Data.Cqrs
{
	public interface IServiceLocator
	{
		/// <summary>
		/// Gets an instance of the specified service.
		/// </summary>
		/// <typeparam name="TService">The service to resolve.</typeparam>
		/// <returns>An instance of the service.</returns>
		TService GetInstance<TService>();

		/// <summary>
		/// Gets an instance of the specified service.
		/// </summary>
		/// <param name="service">The service to resolve.</param>
		/// <returns>An instance of the service.</returns>
		Object GetInstance(Type service);

		/// <summary>
		/// Tries to get an instance of the specified service.
		/// </summary>
		/// <typeparam name="TService">The service to resolve.</typeparam>
		/// <returns>An instance of the service, or null if no implementation was available.</returns>
		TService TryGetInstance<TService>()
			where TService : class;

		/// <summary>
		/// Tries to get an instance of the specified service.
		/// </summary>
		/// <param name="service">The service to resolve.</param>
		/// <returns>An instance of the service, or null if no implementation was available.</returns>
		Object TryGetInstance(Type service);

		//IEnumerable<Object> GetAllInstances(Type t);

		//IEnumerable<TService> GetAllInstances<TService>();
	}
}