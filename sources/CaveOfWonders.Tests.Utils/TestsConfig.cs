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
		return GetConfigs(Root.Value.GetProperty(portName));
	}

	/// <summary>
	/// Same as <see cref="GetPortTestConfigs"/>, but reads from the nested "Environments" section, which declares
	/// the <see cref="ITestEnvironment{TSut,TGateway}"/> types to test each port's adapters through, instead of the
	/// plain <see cref="ISutFixture{T}"/> types.
	/// </summary>
	public static IEnumerable<PortTestConfig> GetEnvironmentTestConfigs(string portName)
	{
		JsonElement environmentsSection = Root.Value.GetProperty("Environments");
		return GetConfigs(environmentsSection.GetProperty(portName));
	}

	private static IEnumerable<PortTestConfig> GetConfigs(JsonElement section)
	{
		foreach (JsonElement item in section.EnumerateArray())
		{
			string typeName = item.GetProperty("type").GetString();
			Type type = Type.GetType(typeName, throwOnError: true);
			yield return new PortTestConfig(type, item);
		}
	}

	private static JsonElement LoadRoot()
	{
		string path = Path.Combine(AppContext.BaseDirectory, "tests-config.json");
		string json = File.ReadAllText(path);
		return JsonDocument.Parse(json).RootElement;
	}
}
