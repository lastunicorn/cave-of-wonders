namespace DustInTheWind.CaveOfWonders.Ports.SystemAccess;

public interface ISystemClock
{
    public DateTime Now { get; }

    public DateTime Today { get; }
}