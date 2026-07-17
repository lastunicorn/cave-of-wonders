using System.Reflection;

namespace DustInTheWind.CaveOfWonders.Tests.Utils;

/// <summary>
/// Locates and instantiates the <see cref="ITestEnvironment{TSut,TGateway}"/> implementation, from a given
/// assembly, that is decorated with a <see cref="TestEnvironmentAttribute"/> matching the requested label.
/// </summary>
public static class TestEnvironmentFactory
{
	public static ITestEnvironment<TSut, TGateway> Create<TSut, TGateway>(Assembly assembly, string label)
	{
		Type environmentType = assembly.GetTypes()
			.SingleOrDefault(type => typeof(ITestEnvironment<TSut, TGateway>).IsAssignableFrom(type) &&
				type.GetCustomAttribute<TestEnvironmentAttribute>()?.Label == label);

		if (environmentType is null)
		{
			throw new InvalidOperationException(
				$"No {nameof(ITestEnvironment<TSut, TGateway>)} implementation tagged with " +
				$"[{nameof(TestEnvironmentAttribute)}(\"{label}\")] was found in assembly '{assembly.GetName().Name}'.");
		}

		return (ITestEnvironment<TSut, TGateway>)Activator.CreateInstance(environmentType);
	}
}
