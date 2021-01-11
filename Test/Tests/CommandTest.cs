using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using AutoMapper;

using Kovi.Data.Cqrs;
using EF = Data.Cqrs.Test.EF;
using Dapper = Data.Cqrs.Test.Dapper;

using Ninject;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Data.Cqrs.Test.EF;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics;

namespace Data.Cqrs.Test
{
	[TestClass]
	public class CommandTest
	{
		private readonly IServiceLocator serviceLocator;
		private readonly ICqService _cqService;

		public CommandTest()
		{
			IKernel ninject = new StandardKernel(new FactoriesModule(), new QueriesModule(), new AutoMapperModule());
			serviceLocator = new NinjectServiceLocator(ninject);
			//_cqService = new CqServiceLocator(serviceLocator);

			//IUnitOfWorkFactory uowFactory = ninject.Get<IUnitOfWorkFactory>();

			// инициализируем контекст
			//System.Data.Entity.Database.SetInitializer(new BooksInitializer());
			//using (BookContext ctx = new BookContext())
			//	ctx.Database.Initialize(true);

		}

		[TestMethod]
		public void ResultTest()
		{
			var failure1 = new Failure("Test1");
			var failure2 = new Failure("Test2");
			var failure = new Failure(failure1, failure2);

			Assert.AreEqual(failure1.Message + Environment.NewLine + failure2.Message, failure.Message);

			//Result res = failure;
			//var res1 = Result.Fail(failure);

			
		}

		[TestMethod]
		public void BookCommandTest()
		{
			var cmd = new CreateBookCmd()
			{
				Name = "Уход за крысами",
				Genres = new int[] { 2 },
				Authors = new int[]{ 2 },
				HasElectronic = false,
				Published = 5018
			};
			
			var res = cmd.Validate(out IEnumerable<ValidationResult> errors);
			Assert.IsFalse(res);
			Assert.AreEqual(3, errors.Count());

			cmd.Published = DateTime.Now.Year-1;

			Result result;
			result = _cqService.Process(cmd);
			Assert.IsTrue(result.IsSuccess);
			//Console.WriteLine(result.Return(r => r.ToString(), r => r.ToString()));

			result = _cqService.Process(cmd);
			Assert.IsFalse(result.IsSuccess);
			//Console.WriteLine(result.Return(r => r.ToString(), r => r.ToString()));

		}


		[TestMethod]
		public void AttrValidatorTest()
		{
			CreateBookCmd cmd = new CreateBookCmd()
			{
				Name = "   ",
				Genres = new int[] { 4 },
				HasElectronic = true,
				Published = DateTime.Now.Year+10,
				Authors = new int[] { 4 }
			};
			var res = cmd.Validate(out IEnumerable<ValidationResult> errors);
			foreach (var e in errors)
				Debug.WriteLine(e.ErrorMessage);
			Assert.IsFalse(res);
			Assert.AreEqual(2, errors.Count());

		}

	}

}
