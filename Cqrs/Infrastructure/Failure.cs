using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kovi.Data.Cqrs
{
	/// <summary>
	/// Информация о результате выполнения
	/// </summary>
	public class Failure
	{
		/// <summary>
		/// Дефолтное значение неудачи
		/// </summary>
		public const Int32 Fault = -1;

		/// <summary>
		/// Сообщение
		/// </summary>
		public String Message { get; }

		/// <summary>
		/// Код результата
		/// </summary>
		public Int32 Code { get; }

		/// <summary>
		/// Результаты
		/// </summary>
		public ReadOnlyDictionary<String, Object> Data { get; protected set; }

		#region .ctor

		public Failure(String message, Int32 errorCode)
		{
			if (errorCode == 0)
			{
				throw new ArgumentException("The error code must be different from 0", nameof(errorCode));
			}
			Code = errorCode;
			Message = message;
		}

		public Failure(String message, Int32 errorCode, IDictionary<String, Object> data)
			: this(message, errorCode)
		{
			Data = new ReadOnlyDictionary<String, Object>(data);
		}

		public Failure(String message, Int32 errorCode, Object data)
			: this(message, errorCode, new Dictionary<String, Object>{{ String.Empty, data }})
		{ }

		public Failure(String message, Object data)
			: this(message, Fault, new Dictionary<String, Object> { { String.Empty, data } })
		{ }

		public Failure(String message, Int32 errorCode, IEnumerable<Object> data)
			: this(message, errorCode)
		{
			var dict = new Dictionary<String, Object>();
			int i = 1;
			foreach (var obj in data)
			{
				dict.Add(i++.ToString(), obj);
			}

			Data = new ReadOnlyDictionary<String, Object>(dict);
		}

		public Failure(Int32 errorCode)
			:this(null, errorCode)
		{ }

		public Failure(String message)
			: this(message, Fault)
		{ }

		public Failure(params Failure[] failures)
			: this(null, Fault)
		{
			if (!failures.Any())
			{
				throw new ArgumentException(nameof(failures));
			}

			Message = String.Join(Environment.NewLine, failures.Select(x => x.Message));

			var dict = new Dictionary<String, Object>();
			for (var i = 0; i < failures.Length; i++)
			{
				dict.Add((i + 1).ToString(), failures[i]);
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