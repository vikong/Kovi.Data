using System;
using System.Collections.Generic;
using Autofac;
using Data.Cqrs.Test.Dapper;
using Kovi.Autofac;
using Kovi.Data.Cqrs;
using Kovi.Data.Cqrs.Infrastructure;
using Kovi.Data.Dapper;

namespace Data.Cqrs.Test
{
	public class DapperAutofacModule : Autofac.Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder.RegisterType<AutofacInstanceFactory>()
				.As<IInstanceFactory>()
				.SingleInstance();

			// сервис
			builder.RegisterType<HandlerLocator>()
				.As<ICqHandlerService>()
				.SingleInstance();

			builder.RegisterGeneric(typeof(DapperQueryHandler<,>))
				.As(typeof(IDapperQueryHandler<,>));

			// контекст
			builder.RegisterType<BooksConnectionFactory>()
				.As<IConnectionFactory>();

			// запросы
			builder.RegisterType<DapperAllBooksQuery>()
				.As<IQueryHandler<DapperAllFlatBookQriteria, IEnumerable<BookDto>>>();

			builder.RegisterType<BooksDapperMapQuery>()
				.As<IQueryHandler<DapperAllBookQriteria, IEnumerable<BookDto>>>();
		}
	}
}