using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Kovi.Data.Automapper;
using Kovi.Data.Cqrs.Linq;
using Kovi.LinqExtensions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Data.Cqrs.Test.Tests
{
	[TestClass]
	public class ConvertorTests
	{
		private IMapper mapper;
		private readonly IProjector _projector;

		public ConvertorTests()
		{
			var config = new MapperConfiguration(cfg =>
			{
				// автоматическое картирование
				cfg.AddProfile(new AutoMapAttributeProfile(GetType().Assembly));
			});
			mapper = new Mapper(config);
			_projector = new AutoMapperProjector(mapper);
		}

		[TestMethod]
		public void Convertor_Convert_EntitiesToDto()
		{
			var convertor = new EnumLinqConvertor<AuthorDto>(_projector);

			using (var uowStub = Stub.CreateMemoryUoW(typeof(Author)).AddAuthors(5))
			{
				var actual = convertor.Convert(uowStub.Linq.Query<Author>());

				Assert.IsInstanceOfType(actual, typeof(List<AuthorDto>));
			}
		}

		[TestMethod]
		public void Convertor_Convert_DtoToEntities()
		{
			var convertor = new EnumLinqConvertor<Author>(_projector);

			using (var uowStub = Stub.CreateMemoryUoW(typeof(AuthorDto)))
			{
				uowStub.Add(new AuthorDto { Id = 1, Name = "Author1" });
				uowStub.Add(new AuthorDto { Id = 2, Name = "Author2" });
				uowStub.Add(new AuthorDto { Id = 3, Name = "Author3" });

				var actual = convertor.Convert(uowStub.Linq.Query<AuthorDto>());

				Assert.IsInstanceOfType(actual, typeof(List<Author>));
			}
		}

		[TestMethod]
		public void PageConvertor_Convert_EntitiesToDtoPage()
		{
			var convertor = new PageConvertor<AuthorDto>(_projector);

			IPaging paging = new Paging(2,10);

			using (var uowStub = Stub.CreateMemoryUoW(typeof(Author)).AddAuthors(50))
			{
				var actual = convertor.Convert(uowStub.Linq.Query<Author>(), paging);

				Assert.IsInstanceOfType(actual, typeof(Page<AuthorDto>));
			}
		}
		[TestMethod]
		public void QueryCount()
		{

			using (var uowStub = Stub.CreateMemoryUoW(typeof(Author)).AddAuthors(50))
			{
				var q = uowStub.Linq.Query<Author>() as IQueryable;
				var actual = q.Count();

				Assert.AreEqual(50, actual);
			}
		}

	}
}
