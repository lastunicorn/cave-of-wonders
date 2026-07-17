using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.TestEnvironments.JsonEnvironment;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DustInTheWind.CaveOfWonders.Tests.Integration;

/// <summary>
/// Registers this assembly with <see cref="TestEnvironmentFactory"/> so its
/// <see cref="ITestEnvironment{TSut,TGateway}"/> implementations are discoverable, without
/// <see cref="TestEnvironmentFactory"/> needing any built-in knowledge of this project.
/// </summary>
internal static class TestsSetup
{
	[ModuleInitializer]
	internal static void ModuleInitializer()
	{
		Assembly testEnvironmentAssembly = typeof(JsonPotRepositoryEnvironment).Assembly;
		TestEnvironmentFactory.RegisterAssembly(testEnvironmentAssembly);
	}
}
