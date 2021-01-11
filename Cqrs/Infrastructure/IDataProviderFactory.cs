namespace Kovi.Data.Cqrs
{
	public interface IDataProviderFactory
	{
		ILinqProviderFactory LinqFactory { get; }
		IUnitOfWorkFactory UowFactory { get; }
	}
}