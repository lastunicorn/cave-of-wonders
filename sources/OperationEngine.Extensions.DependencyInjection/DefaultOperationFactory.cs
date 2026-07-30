namespace DustInTheWind.OperationEngine.Extensions.DependencyInjection;

internal class DefaultOperationFactory : IOperationFactory
{
	private readonly IServiceProvider serviceProvider;

	public DefaultOperationFactory(IServiceProvider serviceProvider)
	{
		this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
	}

	public TOperation Create<TOperation>()
		where TOperation : IOperation
	{
		return (TOperation)serviceProvider.GetService(typeof(TOperation));
	}

	public TOperation Create<TOperation, TResult>()
		where TOperation : IOperation<TResult>
	{
		return (TOperation)serviceProvider.GetService(typeof(TOperation));
	}
}