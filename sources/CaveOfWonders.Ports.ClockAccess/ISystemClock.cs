namespace DustInTheWind.CaveOfWonders.Ports.ClockAccess;

public interface ISystemClock
{
	DateTime Now { get; }

	DateOnly Today { get; }
}