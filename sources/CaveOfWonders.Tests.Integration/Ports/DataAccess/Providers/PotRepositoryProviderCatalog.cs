using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.Providers;

internal static class PotRepositoryProviderCatalog
{
    public static ISutProvider<IPotRepository> Create(RepositoryProviderConfig config)
    {
        return config.Adapter switch
        {
            "Json" => new JsonPotRepositoryProvider(),
            "LiteDb" => new LiteDbPotRepositoryProvider(),
            _ => throw new NotSupportedException($"Unknown adapter '{config.Adapter}' for {nameof(IPotRepository)}.")
        };
    }
}
