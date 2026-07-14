using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.Repositories;

public class CpiRepository : ICpiRepository
{
    private readonly Database database;

    public CpiRepository(Database database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public Task<IEnumerable<Cpi>> GetAll()
    {
        IEnumerable<Cpi> inflationRecords = database.CpiRecords;
        return Task.FromResult(inflationRecords);
    }

    public Task<Cpi> GetByYear(int year)
    {
        Cpi cpi = database.CpiRecords
            .FirstOrDefault(x => x.Year == year);

        return Task.FromResult(cpi);
    }

    public void Add(Cpi cpi)
    {
        if (cpi == null) throw new ArgumentNullException(nameof(cpi));

        database.CpiRecords.Add(cpi);
    }
}