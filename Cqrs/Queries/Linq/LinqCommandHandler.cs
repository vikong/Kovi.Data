using System;

namespace Kovi.Data.Cqrs.Linq
{

	public class LinqCommandHandler<TIn> 
		: ICommandHandler<TIn>
		where TIn : ICommand
	{
		protected readonly IUnitOfWorkFactory UowFactory;

		protected readonly ILinqCommand<TIn> Command;

		public LinqCommandHandler(ILinqCommand<TIn> command, IUnitOfWorkFactory uowFactory)
		{
			UowFactory = uowFactory;
			Command = command;
		}

		Result IHandler<TIn, Result>.Handle(TIn command)
		{
			string connection = command is IConnection ?
				((IConnection)command).Connection :
				null;

			using (IUnitOfWork uow = UowFactory.Create(connection))
			{
				return Command.Execute(command, uow);
			}

		}
	}


}
