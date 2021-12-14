using System;

namespace Kovi.Data.Cqrs
{
	/// <summary>
	/// Представляет возможности для работы с результатом выполнения
	/// </summary>
	public interface IResult
	{ 

		Boolean IsSuccess { get; }

		Boolean IsFaulted { get; }

		TDestination Return<TDestination>(Func<Object, TDestination> onSuccessFunc, Func<Failure, TDestination> onFailureFunc);

	}

	/// <summary>
	/// 
	/// </summary>
	/// <inheritdoc cref="IResult"/>
	public class Result
		: IResult
	{
		protected internal Failure Failure { get; }

		protected internal Object Value { get; set; }

		#region .ctor

		protected internal Result(Object value, Failure failure)
		{
			Value = value;
			Failure = failure;
		}

		protected internal Result(Failure failure)
			:this(null,failure)
		{ }

		protected internal Result() 
			: this(null, null)
		{ }

		#endregion .ctor

		/// <summary>
		/// "Плохой" результат
		/// </summary>
		public Boolean IsFaulted => Failure != null;

		/// <summary>
		/// Успех
		/// </summary>
		public Boolean IsSuccess => Failure == null;

		//public TDestination Return<TDestination>(TDestination successResult, Func<Failure, TDestination> onFailureFunc)
		//	=> IsSuccess ?
		//	successResult
		//	: onFailureFunc(Failure);

		public TDestination Return<TDestination>(Func<Object, TDestination> onSuccessFunc, Func<Failure, TDestination> onFailureFunc)
			=> IsSuccess ?
			onSuccessFunc(Value)
			: onFailureFunc(Failure);

		#region Static fabric

		/// <summary>
		/// Генерирует результат, свидетельствующий о неудачном выполнении
		/// </summary>
		/// <param name="failure">Объект, содержащий информацию об ошибках</param>
		/// <param name="value">Дополнительное значение</param>
		/// <returns></returns>
		public static Result Fail(Failure failure)
		{
			if (failure == null)
				throw new ArgumentNullException(nameof(failure));

			return new Result(failure);
		}

		/// <summary>
		/// Генерирует результат, свидетельствующий о неудачном выполнении
		/// </summary>
		/// <param name="message">Сообщение</param>
		/// <param name="errorCode">Код ошибки</param>
		public static Result Fail(String message, Int32 errorCode)
		{
			if (message == null)
				throw new ArgumentNullException(nameof(message));

			return new Result(null, new Failure(message, errorCode));
		}

		public static Result Fail(String message, Int32 errorCode, Object value)
		{
			if (message == null)
				throw new ArgumentNullException(nameof(message));

			return new Result(new Failure(message, errorCode, value));
		}

		/// <summary>
		/// Генерирует результат, свидетельствующий о неудачном выполнении
		/// </summary>
		/// <param name="message">Сообщение</param>
		public static Result Fail(String message)
			=> new Result(null, new Failure(message));

		/// <summary>
		/// Генерирует результат, свидетельствующий об успешном выполнении
		/// </summary>
		public static Result Ok()
			=> new Result();

		/// <summary>
		/// Генерирует результат, свидетельствующий об успешном выполнении с дополнительными данными
		/// </summary>
		/// <param name="value">Дополнительные данные</param>
		/// <returns></returns>
		public static Result Ok(Object value)
			=> new Result(value, null);

		#endregion Static fabric

		#region Operators

		public static implicit operator Result(Failure failure)
			=> new Result(null, failure);

		//public static Boolean operator false(Result result) => result.IsFaulted;

		//public static Boolean operator true(Result result) => !result.IsFaulted;

		//public static Result operator &(Result result1, Result result2)
		//	=> Result.Combine(result1, result2);

		//public static Result operator |(Result result1, Result result2)
		//	=> result1.IsFaulted ? result2 : result1;

		#endregion Operators
	}

	//public class Result<T> : Result
	//{
	//	protected internal T Value { get; private set; }

	//	#region .ctor

	//	internal Result(T value, Failure failure) : base(failure)
	//	{
	//		Value = value;
	//	}

	//	#endregion .ctor

	//	public TDestination Return<TDestination>(Func<T, TDestination> onSuccessFunc, Func<Failure, TDestination> onFailureFunc)
	//		=> IsSuccess ?
	//		onSuccessFunc(Value)
	//		: onFailureFunc(Failure);

	//}

	public static class ResultExtensions
	{
		public static Result OnSuccess(this Result result, Func<Result> func)
			=> result.IsFaulted ?
				result
				: func();

		public static Result OnSuccess(this Result result, Func<Object, Result> func)
			=> result.IsFaulted ?
				result
				: func(result.Value);

		public static Result OnSuccess(this Result result, Action action)
		{
			if (result.IsFaulted)
				return result;

			action();

			return Result.Ok();
		}

		//public static Result<T> OnSuccess<T>(this Result result, Func<T> func)
		//{
		//	if (result.IsFaulted)
		//		return Result.Fail<T>(result.Failure);

		//	return Result.Ok(func());
		//}

		//public static Result<T> OnSuccess<T>(this Result result, Func<Result<T>> func)
		//	=> result.IsFaulted
		//		? Result.Fail<T>(result.Failure)
		//		: func();

		//public static Result OnSuccess<T>(this Result<T> result, Action<T> action)
		//{
		//	if (result.IsFaulted)
		//		return result;

		//	action(result.Value);

		//	return Result.Ok();
		//}

		//public static Result OnSuccess<T>(this Result<T> result, Func<T, Result> func)
		//{
		//	if (result.IsFaulted)
		//		return result;

		//	return func(result.Value);
		//}

		public static Result OnFailure(this Result result, Action action)
		{
			if (result.IsFaulted)
			{
				action();
			}

			return result;
		}

		public static Result OnBoth(this Result result, Action<Result> action)
		{
			action(result);

			return result;
		}

		//public static T OnBoth<T>(this Result<T> result, Func<Result<T>, T> func)
		//	=> func(result);

		//public static Result<TDestination> Select<TSource, TDestination>(
		//	this Result<TSource> source,
		//	Func<TSource, Result<TDestination>> selector)
		//	=> source.IsFaulted
		//		? new Result<TDestination>(source.Failure)
		//		: selector(source.Value);

		//public static Result<TDestination> SelectMany<TSource, TDestination>(
		//	this Result<TSource> source,
		//	Func<TSource, Result<TDestination>> selector)
		//	=> source.IsFaulted
		//		? new Result<TDestination>(source.Failure)
		//		: selector(source.Value);

		//public static Result<TDestination>
		//	SelectMany<TSource, TIntermediate, TDestination>(
		//	this Result<TSource> result,
		//	Func<TSource, Result<TIntermediate>> inermidiateSelector,
		//	Func<TSource, TIntermediate, Result<TDestination>> resultSelector)
		//	=> result.SelectMany<TSource, TDestination>(s => inermidiateSelector(s)
		//		.SelectMany<TIntermediate, TDestination>(m => resultSelector(s, m)));
	}
}