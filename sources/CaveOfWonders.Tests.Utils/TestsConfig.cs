using System.Text.Json;

namespace DustInTheWind.CaveOfWonders.Tests.Utils;

/// <summary>
/// Loads <c>tests-config.json</c> from the test run's base directory. The file declares, per port
/// interface, the labels (see <see cref="TestEnvironmentAttribute"/>) of the
/// <see cref="ITestEnvironment{TSut,TGateway}"/> implementations to run the interface's test suite against.
/// </summary>
public static class TestsConfig
{
	private static readonly Lazy<JsonElement> Root = new(LoadRoot);

	public static IEnumerable<PortTestConfig> GetEnvironmentTestConfigs(string portName)
	{
		return GetConfigs(Root.Value.GetProperty(portName));
	}

	private static IEnumerable<PortTestConfig> GetConfigs(JsonElement section)
	{
		foreach (JsonElement item in section.EnumerateArray())
		{
			string label = item.GetProperty("label").GetString();
			yield return new PortTestConfig(label, item);
		}
	}

	private static JsonElement LoadRoot()
	{
		string path = Path.Combine(AppContext.BaseDirectory, "tests-config.json");
		string json = File.ReadAllText(path);
		return JsonDocument.Parse(json).RootElement;
	}
}
