using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess;

public class PotRepository : IPotRepository
{
    private readonly Database database;

    public PotRepository(Database database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public Task<IEnumerable<Pot>> GetAll()
    {
        IEnumerable<Pot> result = database.Pots;
        return Task.FromResult(result);
    }

    public Task<IEnumerable<PotSnapshot>> GetSnapshot(DateTime date)
    {
        IEnumerable<PotSnapshot> potSnapshots = database.Pots
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