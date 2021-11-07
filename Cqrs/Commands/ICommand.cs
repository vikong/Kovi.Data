using System;
using System.Threading.Tasks;

namespace Kovi.Data.Cqrs
{
	/// <summary>
	/// Индикатор данных команды.
	/// </summary>
	public interface ICommand : IQriteria
	{ }

	[Obsolete]
	public interface ICmdParam : IQriteria
	{ }

	[Obsolete]
	public interface ICommand<in TParam>
		where TParam : ICommand
	{
		Result Execute(TParam param, String context = null);
	}

	[Obsolete]
	public interface IAsyncCommand<in TParam>
		where TParam : ICommand
	{
		Task<Result> Execute(TParam param, String context = null);
	}
}