using System;
using System.Threading.Tasks;

using Kovi.Data.Cqrs;

namespace Kovi.Data.Dapper
{
	/// <summary>
	/// Базовый класс для запросов с использованием Dapper.
	/// </summary>
	/// <typeparam name="TIn">Тип критерия запроса</typeparam>
	/// <typeparam name="TOut">Тип результата выполнения запроса</typeparam>
	/// <inheritdoc cref="IDapperQuery{TIn, TOut}"/>
	public abstract class BaseDapperQuery<TIn, TOut>
		: IDapperQuery<TIn, TOut>
		where TIn : IQriteria
	{
		public abstract String Sql { get; }

		protected TIn qrit;

		public Object QueryParams => qrit;

		private readonly IQueryObjectHandler<TOut> handler;

		public BaseDapperQuery(IQueryObjectHandler<TOut> handler)
		{
			this.handler = handler;
		}

		public virtual TOut Handle(TIn qrit)
		{
			this.qrit = qrit;
			return handler.Handle(this);
		}

		public override string ToString() => Sql;
	}
}