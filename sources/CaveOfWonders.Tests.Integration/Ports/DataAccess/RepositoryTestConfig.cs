using System.Text.Json;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess;

/// <summary>
/// Loads <c>tests-config.json</c>, which declares, per repository interface, the fully-qualified SUT
/// fixture types to run the interface's test suite against.
/// </summary>
internal static class RepositoryTestConfig
{
	private static readonly Lazy<JsonElement> Root = new(LoadRoot);

	public static IEnumerable<PortTestConfig> GetPortTestConfigs(string portName)
	{
		JsonElement section = Root.Value.GetProperty(portName);

		foreach (JsonElement item in section.EnumerateArray())
		{
			string typeName = item.GetProperty("type").GetString();
			Type fixtureType = Type.GetType(typeName, throwOnError: true);
			yield return new PortTestConfig(fixtureType, item);
		}
	}

	private static JsonElement LoadRoot()
	{
		string path = Path.Combine(AppContext.BaseDirectory, "tests-config.json");
		string json = File.ReadAllText(path);
		return JsonDocument.Parse(json).RootElement;
	}
}