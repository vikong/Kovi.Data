using System;
using System.Collections.Generic;
using Autofac;
using Data.Cqrs.Test.EF;
using Kovi.Autofac;
using Kovi.Data.Cqrs;
using Kovi.Data.Cqrs.Infrastructure;
using Kovi.Data.Cqrs.Linq;
using Kovi.Data.EF;

namespace Data.Cqrs.Test
{
	public class EfAutofacModule : Autofac.Module
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

			builder.RegisterGeneric(typeof(LinqQueryHandler<,>))
				.As(typeof(IQueryHandler<,>));

			builder.RegisterGeneric(typeof(LinqCommandHandler<>))
				.As(typeof(ICommandHandler<>));

			// контекст
			builder.RegisterType<BookContextFactory>()
				.As<IDbContextFactory>();

			builder.RegisterType<EFLinqProviderFactory>()
				.As<ILinqProviderFactory>()
				.SingleInstance();

			builder.RegisterType<EFUnitOfWorkFactory>()
				.As<IUnitOfWorkFactory>()
				.SingleInstance();

			// команды
			builder.RegisterType<CreateAuthorCommand>()
				.As<ILinqCommand<CreateAuthorCmd>>();

			// запросы
			builder.RegisterType<SimpleQuery>()
				.As<IQueryHandler<StringQriteria, QueryResult<String>>>();

			builder.RegisterType<AllAuthorLinqQuery>()
				 .As<ILinqQuery<LinqQriteria, IEnumerable<Author>>>();

			builder.RegisterType<AuthorLinqQuery>()
				 .As<ILinqQuery<NameQriteria, IEnumerable<Author>>>();

		}
	}
}