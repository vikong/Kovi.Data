
using AutoMapper;

using Kovi.Data.Automapper;

using Ninject;
using Ninject.Modules;

namespace Data.Cqrs.Test
{
	public class AutoMapperModule : NinjectModule
	{
		public override void Load()
		{
			var mapperConfiguration = CreateConfiguration();

			Bind<MapperConfiguration>().ToConstant(mapperConfiguration).InSingletonScope();

			Bind<IMapper>().ToMethod(ctx =>
				 new Mapper(mapperConfiguration, type => ctx.Kernel.Get(type)));
		}

		private MapperConfiguration CreateConfiguration()
		{
			//загрузим типы с атрибутом проекций
			//	AppDomain.CurrentDomain.GetAssemblies()
			//	.SingleOrDefault(a=>a.GetName().Name=="Data.Cqrs.Test");
			//GetType().Assembly;

			var config = new MapperConfiguration(cfg =>
			{
				
				// автоматическое картирование
				cfg.AddProfile(new AutoMapAttributeProfile(GetType().Assembly));

				// Add all profiles in current assembly
				//cfg.AddProfiles(GetType().Assembly);
			});

			return config;
		}
	}

}
