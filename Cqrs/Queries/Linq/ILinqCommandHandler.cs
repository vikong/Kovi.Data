namespace Kovi.Data.Cqrs.Linq
{
	/// <summary>
	/// Представляет обработчик команд с использованием Linq
	/// </summary>
	/// <typeparam name="TIn">Тип команды</typeparam>
	public interface ILinqCommand<TIn>
		where TIn : ICommand
	{
		/// <summary>
		/// Выполняет команду и возвращает результат выполнения
		/// </summary>
		/// <param name="param">команда</param>
		/// <param name="uow">Unit of Work</param>
		/// <returns>результат</returns>
		Result Execute(TIn param, IUnitOfWork uow);
	}
}