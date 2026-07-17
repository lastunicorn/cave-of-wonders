using DustInTheWind.CaveOfWonders.Tests.Utils;
using System.Reflection;
using Xunit.Sdk;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.TestEnvironments;

/// <summary>
/// Supplies one <see cref="ITestEnvironment{TSut,TGateway}"/> per adapter configured for
/// <c>typeof(TSut).Name</c> in <c>tests-config.json</c>, so a <c>[Theory]</c> using this attribute runs once per
/// configured adapter.
/// </summary>
internal class RepositoryEnvironmentsAttribute<TSut, TGateway> : DataAttribute
{
	public override IEnumerable<object[]> GetData(MethodInfo testMethod)
	{
		IEnumerable<PortTestConfig> configs = TestsConfig.GetEnvironmentTestConfigs(typeof(TSut).Name);

		foreach (PortTestConfig config in configs)
		{
			ITestEnvironment<TSut, TGateway> environment =
				TestEnvironmentFactory.Create<TSut, TGateway>(GetType().Assembly, config.Label);

			yield return [environment];
		}
	}
}
