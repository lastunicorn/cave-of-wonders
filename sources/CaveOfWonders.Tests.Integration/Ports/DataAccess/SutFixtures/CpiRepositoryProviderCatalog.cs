using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures;

internal static class CpiRepositoryProviderCatalog
{
	public static ISutFixture<ICpiRepository> Create(RepositoryProviderConfig config)
	{
		return config.Adapter switch
		{
			"Json" => new JsonCpiRepositoryFixture(),
			"SQLite" => new SqliteCpiRepositoryFixture(),
			_ => throw new NotSupportedException($"Unknown adapter '{config.Adapter}' for {nameof(ICpiRepository)}.")
		};
	}
}
