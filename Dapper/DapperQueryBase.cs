using System;
using System.Collections.Generic;
using Kovi.Data.Cqrs;

namespace Kovi.Data.Dapper
{
	/// <summary>
	/// Шаблон для запросов Dapper
	/// </summary>
	/// <typeparam name="TIn">Тип параметров запроса</typeparam>
	/// <typeparam name="T">Тип данных, возвращаемых Dapper</typeparam>
	/// <typeparam name="TOut">Тип возвращаемых данных</typeparam>
	/// <inheritdoc/>
	public abstract class DapperQueryBase<TIn, T, TOut>
		: IQueryHandler<TIn, TOut>
		, IDapperQuery<TIn, T, TOut>
		where TIn : IQriteria
	{
		protected readonly IDapperQueryHandler<TIn, TOut> QueryHandler;

		public abstract String Sql { get; }

		public TIn Param { get; protected set; }

		public DapperQueryBase(IDapperQueryHandler<TIn, TOut> queryHandler)
		{
			QueryHandler = queryHandler ?? throw new ArgumentNullException(nameof(queryHandler));
		}

		/// <summary>
		/// Выполняет запрос
		/// </summary>
		/// <param name="qrit">Параметры запроса</param>
		/// <returns>Данные, возвращаемые в результате запроса</returns>
		public virtual TOut Handle(TIn qrit)
		{
			Param = qrit;
			return QueryHandler.Handle(this);
		}

		public abstract TOut Convert(IEnumerable<T> buffer);
	}

}