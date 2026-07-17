namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.ClockAccess.SystemClockTests.TestEnvironments;

internal sealed class RealSystemClockGateway : ISystemClockGateway
{
	public DateTime GetCurrentTime()
	{
		return DateTime.Now;
	}

	public DateOnly GetCurrentDate()
	{
		return DateOnly.FromDateTime(DateTime.Today);
	}
}
