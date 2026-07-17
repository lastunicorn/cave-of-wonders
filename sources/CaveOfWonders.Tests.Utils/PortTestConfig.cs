using System.Text.Json;

namespace DustInTheWind.CaveOfWonders.Tests.Utils;

/// <summary>
/// One entry from <c>tests-config.json</c>: the <see cref="ITestEnvironment{TSut,TGateway}"/> type to test
/// through, plus any adapter-specific settings.
/// </summary>
public readonly struct PortTestConfig
{
	public Type AdaptorType { get; }

	public JsonElement Settings { get; }

	public PortTestConfig(Type adaptorType, JsonElement settings)
	{
		AdaptorType = adaptorType;
		Settings = settings;
	}
}
