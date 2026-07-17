namespace DustInTheWind.CaveOfWonders.Tests.Utils;

/// <summary>
/// One entry from <c>tests-config.json</c>: a port interface name plus the labels (see
/// <see cref="TestEnvironmentAttribute"/>) of the <see cref="ITestEnvironment{TSut,TGateway}"/> implementations to
/// run that port's test suite against.
/// </summary>
public sealed class PortTestConfig
{
	public string Port { get; init; }

	public List<string> TestEnvironments { get; init; } = [];
}