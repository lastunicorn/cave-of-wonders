namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.ClockAccess.SystemClockTests.TestEnvironments;

/// <summary>
/// Back-door access to the real system clock, used by Arrange and Assert to obtain independent reference
/// timestamps/dates that bound the value returned by the SUT's own <c>ISystemClock.Now</c>/<c>Today</c>, without
/// relying on the SUT's own read path.
/// </summary>
public interface ISystemClockBackDoor
{
	DateTime GetCurrentTime();

	DateOnly GetCurrentDate();
}