using DustInTheWind.CaveOfWonders.Tests.Utils;
using System.Text.Json;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess;

/// <summary>
/// One entry from <c>repositorytests.config.json</c>: the <see cref="ISutFixture{T}"/> type to test, plus any
/// adapter-specific settings.
/// </summary>
internal readonly struct RepositoryProviderConfig
{
	public Type FixtureType { get; }

	public JsonElement Settings { get; }

	public RepositoryProviderConfig(Type fixtureType, JsonElement settings)
	{
		FixtureType = fixtureType;
		Settings = settings;
	}
}