using DustInTheWind.CaveOfWonders.Ports.SystemAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.SystemAccess;

public class SystemClock : ISystemClock
{
    public DateTime Now => DateTime.Now;

    public DateTime Today => DateTime.Today;
}