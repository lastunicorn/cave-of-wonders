using System.Text.Json;

namespace DustInTheWind.CaveOfWonders.Tests.Utils;

/// <summary>
/// Loads <c>tests-config.json</c> from the test run's base directory. The file declares, per port
/// interface, the fully-qualified SUT fixture types to run the interface's test suite against.
/// </summary>
public static class TestsConfig
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
