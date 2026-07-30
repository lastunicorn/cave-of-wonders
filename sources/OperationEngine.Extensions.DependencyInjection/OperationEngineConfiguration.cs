using System.Reflection;

namespace DustInTheWind.OperationEngine.Extensions.DependencyInjection;

public class OperationEngineConfiguration
{
	internal List<Type> OperationTypes { get; private set; } = [];

	internal Type OperationFactoryType { get; private set; }

	public OperationEngineConfiguration AddOperationsFromAssembly(params Assembly[] assemblies)
	{
		if (assemblies == null)
			throw new ArgumentNullException(nameof(assemblies));

		IEnumerable<Type> operationTypes = assemblies
			.Where(x => x != null)
			.SelectMany(x => x.GetTypes())
			.Where(x => x.ImplementsAnyInterface(typeof(IOperation), typeof(IOperation<>), typeof(IStreamOperation<>)));

		OperationTypes.AddRange(operationTypes);

		return this;
	}

	public OperationEngineConfiguration AddOperationsFromAssemblyContaining<T>()
	{
		IEnumerable<Type> operationTypes = typeof(T).Assembly.GetTypes()
			.Where(x => x.ImplementsAnyInterface(typeof(IOperation), typeof(IOperation<>), typeof(IStreamOperation<>)));

		OperationTypes.AddRange(operationTypes);

		return this;
	}

	public OperationEngineConfiguration UseOperationFactory<T>()
		where T : IOperationFactory
	{
		OperationFactoryType = typeof(T);
		return this;
	}
}