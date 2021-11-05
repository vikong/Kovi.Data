using System;

namespace Kovi.Data.Cqrs.Linq
{
	public interface IHasContext
	{
		String Context { get; }
	}


}