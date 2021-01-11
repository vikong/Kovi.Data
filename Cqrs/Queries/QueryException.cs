using System;
using System.Runtime.Serialization;

namespace Kovi.Data.Cqrs
{
	[Serializable]
	public class QueryException : Exception
	{
		public QueryException(String message) : base(message)
		{ }

		public QueryException(String message, Exception innerException) : base(message, innerException)
		{ }

		public QueryException()
		{
		}

		protected QueryException(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			throw new NotImplementedException();
		}
	}
}