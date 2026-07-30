using Microsoft.Extensions.DependencyInjection;

namespace DustInTheWind.OperationEngine.Extensions.DependencyInjection;

public static class ContainerBuilderExtensions
{
	public static void AddOperationEngine(this IServiceCollection builder, Action<OperationEngineConfiguration> configBuilder)
	{
		OperationEngineConfiguration config = new();
		configBuilder?.Invoke(config);

		builder.AddOperations(config.OperationTypes);
		builder.AddOperationFactory(config.OperationFactoryType);
		builder.AddOperationManager();
	}

	private static void AddOperations(this IServiceCollection builder, IEnumerable<Type> operationTypes)
	{
		IEnumerable<Type> types = operationTypes.Where(x => x != null);

		foreach (Type operationType in types)
			builder.AddTransient(operationType);
	}

	private static void AddOperationFactory(this IServiceCollection builder, Type operationFactoryType)
	{
		operationFactoryType ??= typeof(DefaultOperationFactory);

		builder.AddSingleton(typeof(IOperationFactory), operationFactoryType);
	}

	private static void AddOperationManager(this IServiceCollection builder)
	{
		builder.AddSingleton<OperationManager>();
	}
}