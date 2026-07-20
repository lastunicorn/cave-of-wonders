using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Repositories;

internal class AverageWageRepository : IAverageWageRepository
{
    private readonly CaveOfWondersDbContext dbContext;

    public AverageWageRepository(CaveOfWondersDbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IAsyncEnumerable<AverageWage> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.AverageWages
            .OrderBy(x => x.Year)
            .AsAsyncEnumerable();
    }

    public async Task<AverageWage> GetAsync(int year, CancellationToken cancellationToken = default)
    {
        return await dbContext.AverageWages.FindAsync([year], cancellationToken);
    }

    public void Add(AverageWage averageWage)
    {
        ArgumentNullException.ThrowIfNull(averageWage);

        dbContext.AverageWages.Add(averageWage);
    }

    public void Delete(AverageWage averageWage)
    {
        ArgumentNullException.ThrowIfNull(averageWage);

        AverageWage entity = dbContext.AverageWages.Find(averageWage.Year);

        if (entity != null)
            dbContext.AverageWages.Remove(entity);
    }
}
