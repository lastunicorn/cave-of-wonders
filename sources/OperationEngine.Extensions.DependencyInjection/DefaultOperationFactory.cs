namespace DustInTheWind.OperationEngine.Extensions.DependencyInjection;

internal class DefaultOperationFactory : IOperationFactory
{
	private readonly IServiceProvider serviceProvider;

	public DefaultOperationFactory(IServiceProvider serviceProvider)
	{
		this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
	}

	public TOperation Create<TOperation>()
	{
		return (TOperation)serviceProvider.GetService(typeof(TOperation));
	}
}