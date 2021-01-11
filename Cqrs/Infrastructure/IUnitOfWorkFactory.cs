using System;

namespace Kovi.Data.Cqrs
{
	public interface IUnitOfWorkFactory
	{
		IUnitOfWork Create(String connection = null, String context = null);
	}
}