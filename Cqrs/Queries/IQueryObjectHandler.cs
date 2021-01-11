using System;
using System.Threading.Tasks;

namespace Kovi.Data.Cqrs
{
	public interface IQueryObjectHandler<TResponse>
	{
		TResponse Ask(QueryObject query, String source = null);

		Task<TResponse> AskAsync(QueryObject query, String source = null);
	}

}