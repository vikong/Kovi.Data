using System;

namespace Kovi.Data.Cqrs.Linq
{
	public abstract class LinqCommand<TParam> :
		ILinqCommand,
		ICommand<TParam>
		where TParam : ICmdParam
	{
		private readonly ILinqCommandHandler CommandHandler;

		public TParam Cmd { get; protected set; }

		public Result Execute(TParam param, String source = null)
		{
			Cmd = param;
			return CommandHandler.Process(this, source);
		}

		public abstract Result Execute(IUnitOfWork uow);

		#region .ctor

		protected LinqCommand(ILinqCommandHandler commandHandler)
		{
			CommandHandler = commandHandler ?? throw new ArgumentNullException("LinqQueryHandler is null.");
		}

		#endregion .ctor
	}
}