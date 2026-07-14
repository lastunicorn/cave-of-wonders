using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using System.Reflection;
using Xunit.Sdk;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures;

/// <summary>
/// Supplies one <see cref="IPotRepository"/> provider per adapter configured for "IPotRepository" in
/// <c>repositorytests.config.json</c>, so a <c>[Theory]</c> using this attribute runs once per configured adapter.
/// </summary>
internal class PotRepositoryProvidersAttribute : DataAttribute
{
	public override IEnumerable<object[]> GetData(MethodInfo testMethod)
	{
		IEnumerable<RepositoryProviderConfig> configs = RepositoryTestConfig.GetProviders(nameof(IPotRepository));
		
		foreach (RepositoryProviderConfig config in configs)
		{
			ISutFixture<IPotRepository> sutFixture = PotRepositoryProviderCatalog.Create(config);
			yield return [sutFixture];
		}
	}
}