namespace DustInTheWind.CaveOfWonders.Ports.SystemAccess;

public interface ISystemClock
{
    DateTime Now { get; }

    DateOnly Today { get; }
}