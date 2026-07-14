using System.Text.Json;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess;

/// <summary>
/// One entry from <c>repositorytests.config.json</c>: which adapter to test, plus any adapter-specific settings.
/// </summary>
internal readonly struct RepositoryProviderConfig
{
    public string Adapter { get; }

    public JsonElement Settings { get; }

    public RepositoryProviderConfig(string adapter, JsonElement settings)
    {
        Adapter = adapter;
        Settings = settings;
    }
}
