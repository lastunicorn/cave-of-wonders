using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.Gateways;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using System.Reflection;
using Xunit.Sdk;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures;

/// <summary>
/// Supplies one <see cref="ITestEnvironment{TSut,TGateway}"/> per adapter configured for "ICpiRepository" in the
/// "Environments" section of <c>tests-config.json</c>, so a <c>[Theory]</c> using this attribute runs once per
/// configured adapter.
/// </summary>
internal class CpiRepositoryEnvironmentsAttribute : DataAttribute
{
	public override IEnumerable<object[]> GetData(MethodInfo testMethod)
	{
		IEnumerable<PortTestConfig> configs = TestsConfig.GetEnvironmentTestConfigs(nameof(ICpiRepository));

		foreach (PortTestConfig config in configs)
		{
			ITestEnvironment<ICpiRepository, ICpiStorageGateway> environment =
				(ITestEnvironment<ICpiRepository, ICpiStorageGateway>)Activator.CreateInstance(config.AdaptorType);
			yield return [environment];
		}
	}
}
