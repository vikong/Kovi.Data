using System;
using System.Threading.Tasks;

namespace Kovi.Data.Cqrs
{
	/// <summary>
	/// Индикатор данных команды.
	/// </summary>
	public interface ICmdParam : IQriteria
	{ }

	public interface ICommand<in TParam>
		where TParam : ICmdParam
	{
		Result Execute(TParam param, String context = null);
	}

	public interface IAsyncCommand<in TParam>
		where TParam : ICmdParam
	{
		Task<Result> Execute(TParam param, String context = null);
	}
}