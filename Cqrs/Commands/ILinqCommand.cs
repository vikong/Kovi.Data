using System;

namespace Kovi.Data.Cqrs.Linq
{
	public interface ILinqCommand
	{
		Result Execute(IUnitOfWork uow);
	}

	//public interface ILinqCommandAsync
	//{
	//	Task<Result> Execute(IUnitOfWork uow);
	//}

	public interface ILinqCommandHandler
	{
		Result Process(ILinqCommand command, String source = null);

		//Task<Result> Process(ILinqCommandAsync command, String source = null);
	}


}