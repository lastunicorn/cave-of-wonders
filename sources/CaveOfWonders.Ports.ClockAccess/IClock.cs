namespace DustInTheWind.CaveOfWonders.Ports.ClockAccess;

public interface IClock
{
    DateTime Now { get; }

    DateOnly Today { get; }
}