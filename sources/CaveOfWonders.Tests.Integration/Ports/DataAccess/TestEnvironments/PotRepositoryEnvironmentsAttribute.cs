using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using System.Reflection;
using Xunit.Sdk;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.TestEnvironments;

/// <summary>
/// Supplies one <see cref="ITestEnvironment{TSut,TGateway}"/> per adapter configured for "IPotRepository" in the
/// "Environments" section of <c>tests-config.json</c>, so a <c>[Theory]</c> using this attribute runs once per
/// configured adapter.
/// </summary>
internal class PotRepositoryEnvironmentsAttribute : DataAttribute
{
	public override IEnumerable<object[]> GetData(MethodInfo testMethod)
	{
		IEnumerable<PortTestConfig> configs = TestsConfig.GetEnvironmentTestConfigs(nameof(IPotRepository));

		foreach (PortTestConfig config in configs)
		{
			ITestEnvironment<IPotRepository, IPotStorageGateway> environment =
				(ITestEnvironment<IPotRepository, IPotStorageGateway>)Activator.CreateInstance(config.AdaptorType);
			
			yield return [environment];
		}
	}
}
