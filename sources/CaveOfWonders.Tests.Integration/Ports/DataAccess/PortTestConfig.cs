using DustInTheWind.CaveOfWonders.Tests.Utils;
using System.Text.Json;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess;

/// <summary>
/// One entry from <c>tests-config.json</c>: the <see cref="ISutFixture{T}"/> type to test, plus any
/// adapter-specific settings.
/// </summary>
internal readonly struct PortTestConfig
{
	public Type AdaptorType { get; }

	public JsonElement Settings { get; }

	public PortTestConfig(Type adaptorType, JsonElement settings)
	{
		AdaptorType = adaptorType;
		Settings = settings;
	}
}