using DustInTheWind.OperationEngine;

namespace CaveOfWonders.Tests;

/// <summary>
/// An <see cref="IOperationFactory"/> that creates operations using explicitly registered factory methods,
/// allowing the test to provide the mocked dependencies of each operation.
/// </summary>
internal class TestOperationFactory : IOperationFactory
{
	private readonly Dictionary<Type, Func<object>> factories = [];

	public void Register<TOperation>(Func<TOperation> factory)
	{
		factories.Add(typeof(TOperation), () => factory());
	}

	public TOperation Create<TOperation>()
		where TOperation : IOperation
	{
		return (TOperation)CreateOperation<TOperation>();
	}

	public TOperation Create<TOperation, TResult>()
		where TOperation : IOperation<TResult>
	{
		return (TOperation)CreateOperation<TOperation>();
	}

	private object CreateOperation<TOperation>()
	{
		if (!factories.TryGetValue(typeof(TOperation), out Func<object> factory))
			throw new InvalidOperationException($"No factory method was registered for operation {typeof(TOperation).Name}.");

		return factory();
	}
}
