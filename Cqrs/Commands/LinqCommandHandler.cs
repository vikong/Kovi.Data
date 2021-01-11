using System;
using System.Text;

namespace Kovi.Data.Cqrs.Linq
{
	public class LinqCommandHandler
		: ILinqCommandHandler
	{
		private readonly IUnitOfWorkFactory _uowFactory;

		protected static Exception BuildException(Object command, Exception ex, IUnitOfWork uow = null, String message = null)
		{
			var sb = new StringBuilder("Error during handle command. ");
			sb.AppendLine(message ?? String.Empty)
				.AppendLine(command.ToString())
				.Append(uow != null ? uow.ToString() : String.Empty);

			return new Exception(sb.ToString(), ex);
		}

		Result ILinqCommandHandler.Process(ILinqCommand command, String source)
		{
			try
			{
				using (IUnitOfWork uow = _uowFactory.Create(source))
				{
					Result result = command.Execute(uow);
					if (result.IsSuccess)
						uow.Commit();
					return result;
				}
			}
			catch (Exception ex)
			{
				throw BuildException(command, ex);
			}
		}

		#region .ctor

		public LinqCommandHandler(IUnitOfWorkFactory uowFactory)
		{
			_uowFactory = uowFactory;
		}

		#endregion .ctor
	}
}