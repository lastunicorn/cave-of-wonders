using System.Reflection;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using Xunit.Sdk;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.Providers;

/// <summary>
/// Supplies one <see cref="IPotRepository"/> provider per adapter configured for "IPotRepository" in
/// <c>repositorytests.config.json</c>, so a <c>[Theory]</c> using this attribute runs once per configured adapter.
/// </summary>
internal class PotRepositoryProvidersAttribute : DataAttribute
{
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        foreach (RepositoryProviderConfig config in RepositoryTestConfig.GetProviders(nameof(IPotRepository)))
            yield return [PotRepositoryProviderCatalog.Create(config)];
    }
}
