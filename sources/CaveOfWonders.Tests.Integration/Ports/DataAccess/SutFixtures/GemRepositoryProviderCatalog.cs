using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures;

internal static class GemRepositoryProviderCatalog
{
	public static IGemRepositorySutFixture Create(RepositoryProviderConfig config)
	{
		return config.Adapter switch
		{
			"Json" => new JsonGemRepositoryFixture(),
			"SQLite" => new SqliteGemRepositoryFixture(),
			_ => throw new NotSupportedException($"Unknown adapter '{config.Adapter}' for {nameof(IGemRepository)}.")
		};
	}
}
