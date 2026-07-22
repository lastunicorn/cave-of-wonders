using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;

internal static class PotExtensions
{
    public static PotSnapshot GetSnapshot(this Pot pot, DateOnly date, DateMatchingMode dateMatchingMode)
    {
        return dateMatchingMode switch
        {
            DateMatchingMode.Exact => pot.Snapshots.FirstOrDefault(x => x.Date == date),
            DateMatchingMode.LastAvailable => pot.Snapshots
                .Where(x => x.Date <= date)
                .MaxBy(x => x.Date),
            _ => throw new ArgumentOutOfRangeException(nameof(dateMatchingMode))
        };
    }
}