namespace Kovi.Data.Dapper
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Представляет запрос, который использует ORM Dapper для получения данных
	/// </summary>
	/// <typeparam name="TIn">Тип параметров запроса</typeparam>
	/// <typeparam name="T">Тип данных, возвращаемых Dapper</typeparam>
	/// <typeparam name="TOut">Тип возвращаемых данных</typeparam>
	public interface IDapperQuery<TIn, T, TOut>
	{
		/// <summary>
		/// Запрос SQL для выполнения
		/// </summary>
		String Sql { get; }

		/// <summary>
		/// Параметры запроса
		/// </summary>
		TIn Param { get; }

		/// <summary>
		/// Функция конвертации данных в результат
		/// </summary>
		/// <param name="buffer">Данные, возращаемые Dapper</param>
		/// <returns></returns>
		TOut Convert(IEnumerable<T> buffer);

	}

	/// <summary>
	/// Представляет возможности для конвертации данных, получаемых Dapper из БД
	/// </summary>
	/// <typeparam name="TIn">Тип параметров запроса</typeparam>
	/// <typeparam name="T">Тип данных, возвращаемых Dapper</typeparam>
	/// <typeparam name="TOut">Тип возвращаемых данных</typeparam>
	// Данные, полученные в результате запроса, представляются в виде массива объектов типов <see cref="SplitTypes"/>
	// Поля типов заполняются данными из колонок, разделённых согласно <see cref="SplitOn"/>
	// Каждая запись, представленная в виде массива экземпляров типов, передаётся в <see cref="MapFunc(object[])"/>
	public interface IDapperMapQuery<TIn, T, TOut>
		: IDapperQuery<TIn, T, TOut>
	{
		/// <summary>
		/// Типы, в которые мапируются данные, получаемые Dapper из БД
		/// </summary>
		Type[] SplitTypes { get; }

		/// <summary>
		/// Имена столбцов для разделения таблицы на <see cref="SplitTypes"/>, через запятую
		/// </summary>
		String SplitOn { get; }

		/// <summary>
		/// Функция, конвертирующая данные, получаемые Dapper из БД
		/// </summary>
		/// <param name="par">Запись, представленная массивом типов <see cref="SplitTypes"/></param>
		/// <returns></returns>
		T MapFunc(Object[] par);
	}


}