using System.Text.Json;

namespace DustInTheWind.CaveOfWonders.Tests.Utils;

/// <summary>
/// One entry from <c>tests-config.json</c>: the label identifying the <see cref="ITestEnvironment{TSut,TGateway}"/>
/// to test through (see <see cref="TestEnvironmentAttribute"/>), plus any adapter-specific settings.
/// </summary>
public readonly struct PortTestConfig
{
	public string Label { get; }

	public JsonElement Settings { get; }

	public PortTestConfig(string label, JsonElement settings)
	{
		Label = label;
		Settings = settings;
	}
}
