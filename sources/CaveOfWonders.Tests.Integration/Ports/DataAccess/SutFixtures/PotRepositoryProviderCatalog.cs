using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures;

internal static class PotRepositoryProviderCatalog
{
	public static ISutFixture<IPotRepository> Create(RepositoryProviderConfig config)
	{
		return config.Adapter switch
		{
			"Json" => new JsonPotRepositoryFixture(),
			"LiteDb" => new LiteDbPotRepositoryFixture(),
			"SQLite" => new SqlitePotRepositoryFixture(),
			_ => throw new NotSupportedException($"Unknown adapter '{config.Adapter}' for {nameof(IPotRepository)}.")
		};
	}
}