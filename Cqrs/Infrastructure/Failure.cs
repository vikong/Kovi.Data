using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kovi.Data.Cqrs
{
	public class Failure
	{
		public const Int32 Success = 0;
		public const Int32 Fault = -1;
		public String Message { get; }

		public Int32 Code { get; }

		public ReadOnlyDictionary<String, Object> Data { get; protected set; }

		#region .ctor

		public Failure(Int32 errorCode) {
			if (errorCode == Success)
			{
				throw new ArgumentException("The error code must be different from 0", nameof(errorCode));
			}
			Code = errorCode;
		}
		public Failure(String message, Int32 errorCode)
			: this(errorCode)
		{
			Message = message;
		}

		public Failure(String message)
			: this(message, Fault) 
		{ }

		public Failure(params Failure[] failures)
			:this(Fault)
		{
			if (!failures.Any()) { throw new ArgumentException(nameof(failures)); }

			Message = String.Join(Environment.NewLine, failures.Select(x => x.Message));

			var dict = new Dictionary<String, Object>();
			for (var i = 0; i < failures.Length; i++)
			{
				dict.Add((i + 1).ToString(), failures[i]);
			}

			Data = new ReadOnlyDictionary<String, Object>(dict);
		}


		public Failure(String message, Object data)
			: this(message, Fault)
		{
			var dict = new Dictionary<String, Object>
			{
				{ String.Empty, data }
			};

			Data = new ReadOnlyDictionary<String, Object>(dict);
		}

		public Failure(String message, IDictionary<String, Object> data)
			: this(message, Fault)
		{
			Message = message;
			Data = new ReadOnlyDictionary<String, Object>(data);
		}

		public Failure(String message, IEnumerable<Object> data, Int32 errorCode)
			: this(message, errorCode)
		{
			var dict = new Dictionary<String, Object>();
			int i = 1;
			foreach (var o in data)
			{
				dict.Add(i++.ToString(), o);
			}

			Data = new ReadOnlyDictionary<String, Object>(dict);
		}

		#endregion .ctor

		public override String ToString()
			=> $"[({Code}){Message}"
			+ (Data != null ?
			$": \r\n{String.Join(",\r\n ", Data.Select(kvp => kvp.Value.ToString()))}]"
			: "]");
	}
}