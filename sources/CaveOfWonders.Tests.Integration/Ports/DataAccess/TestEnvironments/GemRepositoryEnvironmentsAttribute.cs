using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using System.Reflection;
using Xunit.Sdk;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.TestEnvironments;

/// <summary>
/// Supplies one <see cref="ITestEnvironment{TSut,TGateway}"/> per adapter configured for "IGemRepository" in the
/// "Environments" section of <c>tests-config.json</c>, so a <c>[Theory]</c> using this attribute runs once per
/// configured adapter.
/// </summary>
internal class GemRepositoryEnvironmentsAttribute : DataAttribute
{
	public override IEnumerable<object[]> GetData(MethodInfo testMethod)
	{
		IEnumerable<PortTestConfig> configs = TestsConfig.GetEnvironmentTestConfigs(nameof(IGemRepository));

		foreach (PortTestConfig config in configs)
		{
			ITestEnvironment<IGemRepository, IGemStorageGateway> environment =
				(ITestEnvironment<IGemRepository, IGemStorageGateway>)Activator.CreateInstance(config.AdaptorType);
			
			yield return [environment];
		}
	}
}
