using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using System.Reflection;
using Xunit.Sdk;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures;

/// <summary>
/// Supplies one <see cref="IGemRepositorySutFixture"/> per adapter configured for "IGemRepository" in
/// <c>repositorytests.config.json</c>, so a <c>[Theory]</c> using this attribute runs once per configured adapter.
/// </summary>
internal class GemRepositoryProvidersAttribute : DataAttribute
{
	public override IEnumerable<object[]> GetData(MethodInfo testMethod)
	{
		IEnumerable<RepositoryProviderConfig> configs = RepositoryTestConfig.GetProviders(nameof(IGemRepository));

		foreach (RepositoryProviderConfig config in configs)
		{
			IGemRepositorySutFixture sutFixture = GemRepositoryProviderCatalog.Create(config);
			yield return [sutFixture];
		}
	}
}
