using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using System.Reflection;
using Xunit.Sdk;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures;

/// <summary>
/// Supplies one <see cref="ICpiRepository"/> provider per adapter configured for "ICpiRepository" in
/// <c>tests-config.json</c>, so a <c>[Theory]</c> using this attribute runs once per configured adapter.
/// </summary>
internal class CpiRepositoryProvidersAttribute : DataAttribute
{
	public override IEnumerable<object[]> GetData(MethodInfo testMethod)
	{
		IEnumerable<PortTestConfig> configs = TestsConfig.GetPortTestConfigs(nameof(ICpiRepository));

		foreach (PortTestConfig config in configs)
		{
			ISutFixture<ICpiRepository> sutFixture = (ISutFixture<ICpiRepository>)Activator.CreateInstance(config.AdaptorType);
			yield return [sutFixture];
		}
	}
}
