using System.Text.Json;

namespace DustInTheWind.CaveOfWonders.Tests.Utils;

/// <summary>
/// Loads <c>tests-config.json</c> from the test run's base directory. The file declares, per port
/// interface, the labels (see <see cref="TestEnvironmentAttribute"/>) of the
/// <see cref="ITestEnvironment{TSut,TGateway}"/> implementations to run the interface's test suite against.
/// </summary>
public static class TestsConfig
{
	private static readonly Lazy<List<PortTestConfig>> Root = new(LoadRoot);

	public static IEnumerable<string> GetEnvironmentLabels(string portName)
	{
		return Root.Value
			.Where(x => x.Port == portName)
			.SelectMany(x => x.TestEnvironments);
	}

	private static List<PortTestConfig> LoadRoot()
	{
		string path = Path.Combine(AppContext.BaseDirectory, "tests-config.json");
		string json = File.ReadAllText(path);
		return JsonSerializer.Deserialize<List<PortTestConfig>>(json);
	}
}
