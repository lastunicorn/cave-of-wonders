using DustInTheWind.CaveOfWonders.Ports.ClockAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.ClockAccess;

public class SystemClock : ISystemClock
{
	public DateTime Now => DateTime.Now;

	public DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
}