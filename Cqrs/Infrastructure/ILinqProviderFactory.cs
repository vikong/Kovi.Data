namespace Kovi.Data.Cqrs
{
	using System;

	public interface ILinqProviderFactory
	{
		ILinqProvider Create(String connection = null, String context = null);
	}
}