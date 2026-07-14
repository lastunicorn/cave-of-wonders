using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.Providers;

internal static class PotRepositoryProviderCatalog
{
	public static ISutFixture<IPotRepository> Create(RepositoryProviderConfig config)
	{
		return config.Adapter switch
		{
			"Json" => new JsonPotRepositorySut(),
			"LiteDb" => new LiteDbPotRepositorySut(),
			_ => throw new NotSupportedException($"Unknown adapter '{config.Adapter}' for {nameof(IPotRepository)}.")
		};
	}
}