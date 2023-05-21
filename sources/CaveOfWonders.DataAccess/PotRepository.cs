using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.DataAccess;

public class PotRepository : IPotRepository
{
    public Task<IEnumerable<Pot>> GetAll()
    {
        IEnumerable<Pot> result = Database.Pots;
        return Task.FromResult(result);
    }

    public Task<IEnumerable<PotSnapshot>> GetSnapshot(DateTime date)
    {
        IEnumerable<PotSnapshot> potSnapshots = Database.Pots
            .Where(x => x.IsActive(date))
            .Select(x =>
            {
                Gem gem = x.Gems.FirstOrDefault(z => z.Date == date);

                return new PotSnapshot
                {
                    Pot = x,
                    Gem = gem
                };
            });
        return Task.FromResult(potSnapshots);
    }
}