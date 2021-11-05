using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Cqrs.Test
{
	public class QueryResult<T>
		where T:class
	{
		public List<String> Stack { get; private set; }

		public T Data { get; set; }

		public QueryResult()
		{
			Stack = new List<String>();
		}

		public QueryResult(T data)
			:this()
		{
			Data = data;
		}

		public void From(String from)
		{
			Stack.Add(from);
		}
	}
}
