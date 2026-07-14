using System.Text.Json;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess;

/// <summary>
/// Loads <c>repositorytests.config.json</c>, which declares, per repository interface, which adapters to run the
/// interface's test suite against.
/// </summary>
internal static class RepositoryTestConfig
{
	private static readonly Lazy<JsonElement> Root = new(LoadRoot);

	public static IEnumerable<RepositoryProviderConfig> GetProviders(string repositoryName)
	{
		JsonElement section = Root.Value.GetProperty(repositoryName);

		foreach (JsonElement item in section.EnumerateArray())
		{
			string adapter = item.GetProperty("adapter").GetString();
			yield return new RepositoryProviderConfig(adapter, item);
		}
	}

	private static JsonElement LoadRoot()
	{
		string path = Path.Combine(AppContext.BaseDirectory, "repositorytests.config.json");
		string json = File.ReadAllText(path);
		return JsonDocument.Parse(json).RootElement;
	}
}