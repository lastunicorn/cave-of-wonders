using System.Collections.Concurrent;
using System.Reflection;

namespace DustInTheWind.CaveOfWonders.Tests.Utils;

/// <summary>
/// Locates and instantiates <see cref="ITestEnvironment{TSut,TGateway}"/> implementations decorated with a
/// <see cref="TestEnvironmentAttribute"/> matching a requested label.
/// </summary>
/// <remarks>
/// This class carries no built-in knowledge of any consumer assembly. Every assembly that declares
/// <see cref="ITestEnvironment{TSut,TGateway}"/> implementations must register itself once via
/// <see cref="RegisterAssembly"/> - typically from a
/// <see cref="System.Runtime.CompilerServices.ModuleInitializerAttribute"/> method - before
/// <see cref="Create{TSut,TGateway}"/> is called.
/// </remarks>
public static class TestEnvironmentFactory
{
	private static readonly object RegistrationLock = new();
	private static readonly List<Assembly> RegisteredAssemblies = [];

	private static readonly ConcurrentDictionary<(Type Sut, Type Gateway), Lazy<Dictionary<string, Type>>> EnvironmentsByPortType = new();

	public static void RegisterAssembly(Assembly assembly)
	{
		lock (RegistrationLock)
		{
			if (!RegisteredAssemblies.Contains(assembly))
				RegisteredAssemblies.Add(assembly);
		}
	}

	public static ITestEnvironment<TSut, TGateway> Create<TSut, TGateway>(string label)
	{
		Dictionary<string, Type> environmentsByLabel = EnvironmentsByPortType
			.GetOrAdd((typeof(TSut), typeof(TGateway)), _ => new Lazy<Dictionary<string, Type>>(ScanForEnvironments<TSut, TGateway>))
			.Value;

		if (!environmentsByLabel.TryGetValue(label, out Type environmentType))
		{
			throw new InvalidOperationException(
				$"No {nameof(ITestEnvironment<TSut, TGateway>)} implementation tagged with " +
				$"[{nameof(TestEnvironmentAttribute)}(\"{label}\")] was found in any registered assembly.");
		}

		return (ITestEnvironment<TSut, TGateway>)Activator.CreateInstance(environmentType);
	}

	private static Dictionary<string, Type> ScanForEnvironments<TSut, TGateway>()
	{
		Assembly[] assembliesSnapshot;

		lock (RegistrationLock)
		{
			assembliesSnapshot = RegisteredAssemblies.ToArray();
		}

		Dictionary<string, Type> environmentsByLabel = new();

		foreach (Assembly assembly in assembliesSnapshot)
		{
			foreach (Type type in GetLoadableTypes(assembly))
			{
				if (!typeof(ITestEnvironment<TSut, TGateway>).IsAssignableFrom(type))
					continue;

				string label = type.GetCustomAttribute<TestEnvironmentAttribute>()?.Label;

				if (label is null)
					continue;

				if (environmentsByLabel.TryGetValue(label, out Type existingType))
				{
					throw new InvalidOperationException(
						$"Both '{existingType.FullName}' and '{type.FullName}' are tagged with " +
						$"[{nameof(TestEnvironmentAttribute)}(\"{label}\")] for " +
						$"{nameof(ITestEnvironment<TSut, TGateway>)}.");
				}

				environmentsByLabel.Add(label, type);
			}
		}

		return environmentsByLabel;
	}

	private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
	{
		try
		{
			return assembly.GetTypes();
		}
		catch (ReflectionTypeLoadException ex)
		{
			return ex.Types.Where(type => type is not null);
		}
	}
}