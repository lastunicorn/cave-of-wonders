namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.ClockAccess.SystemClockTests.TestEnvironments;

/// <summary>
/// Back-door access to the real system clock, used by Arrange and Assert to obtain independent reference
/// timestamps that bound the value returned by the SUT's own <c>ISystemClock.Now</c>, without relying on the
/// SUT's own read path.
/// </summary>
public interface ISystemClockGateway
{
	DateTime GetCurrentTime();
}
