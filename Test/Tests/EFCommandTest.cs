using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Data.Cqrs.Test.EF;
using Kovi.Autofac;
using Kovi.Data.Cqrs;
using Kovi.Data.Cqrs.Infrastructure;
using Kovi.Data.Cqrs.Linq;
using Kovi.Data.EF;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Data.Cqrs.Test
{

	[TestClass]
	public class EfCommandTest
		: BaseAutofacTest
	{
		protected override Module AutofacModule => new EfAutofacModule();

		ICqHandlerService hs => Container.Resolve<ICqHandlerService>();

		[TestMethod]
		public void LinqProviderFactory_Creates_Context()
		{
			var linq = Container.Resolve<ILinqProviderFactory>().Create();
			
			Assert.IsInstanceOfType(linq, typeof(EFLinqProvider));
			Assert.IsTrue(linq.Query<Book>().Any());
		}

		[TestMethod]
		public void CreateAuthorCommand_Adds_Author()
		{
			var cmd = new CreateAuthorCmd { Name = "Александропулос" };
			hs.Process(cmd);

			var linq = Container.Resolve<ILinqProviderFactory>().Create();

			Assert.IsTrue(linq.Query<Author>().Any(a => a.Name == cmd.Name));
		}

	}
}