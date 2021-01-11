using System;
using System.Linq;
using System.Threading.Tasks;

using Kovi.LinqExtensions;

namespace Kovi.Data.Cqrs.Linq
{
	///// <summary>
	///// Постраничный конвертор для запросов по конвенции.
	///// </summary>
	///// <typeparam name="TResponse">Тип, возвращаемый страницей.</typeparam>
	//public class ConventionPageConvertor<TResponse>
	//	: IPageConvertor<TResponse>
	//{
	//	private readonly IPageConvertor<TResponse> Convertor;

	//	public ConventionPageConvertor(IProjector projector)
	//	{
	//		if (projector == null)
	//			throw new ArgumentNullException(nameof(projector));



	//		//Type conv = typeof(PageConvertor<,>)
	//		//	.MakeGenericType(new Type[] { Conventions<TResponse>.EntityType, typeof(TResponse) });

	//		//Convertor = (IPageConvertor<TEntity, TResponse>)Activator.CreateInstance(conv, projector);
	//	}

	//	public IPage<TResponse> Convert(IQueryable query, Object param = null)
	//		=> query.Convert(Convertor, param);

	//	public Task<IPage<TResponse>> ConvertAsync(IQueryable query, Object param = null)
	//		=> query.ConvertAsync(Convertor, param);

	//}


}
