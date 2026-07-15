using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using System.Reflection;
using Xunit.Sdk;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures;

/// <summary>
/// Supplies one <see cref="IGemRepositorySutFixture"/> per adapter configured for "IGemRepository" in
/// <c>tests-config.json</c>, so a <c>[Theory]</c> using this attribute runs once per configured adapter.
/// </summary>
internal class GemRepositoryProvidersAttribute : DataAttribute
{
	public override IEnumerable<object[]> GetData(MethodInfo testMethod)
	{
		IEnumerable<PortTestConfig> configs = RepositoryTestConfig.GetPortTestConfigs(nameof(IGemRepository));

		foreach (PortTestConfig config in configs)
		{
			IGemRepositorySutFixture sutFixture = (IGemRepositorySutFixture)Activator.CreateInstance(config.AdaptorType);
			yield return [sutFixture];
		}
	}
}
