using System.Runtime.CompilerServices;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Entities;
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

    public async IAsyncEnumerable<AverageWage> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        List<AverageWageEntity> entities = await dbContext.AverageWages
            .OrderBy(x => x.Year)
            .ToListAsync(cancellationToken);

        foreach (AverageWageEntity entity in entities)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return MapToDomain(entity);
        }
    }

    public async Task<AverageWage> GetAsync(int averageWageYear, CancellationToken cancellationToken)
    {
        AverageWageEntity entity = await dbContext.AverageWages.FindAsync([averageWageYear], cancellationToken);

        return entity == null
            ? null
            : MapToDomain(entity);
    }

    public void Add(AverageWage averageWage)
    {
        ArgumentNullException.ThrowIfNull(averageWage);

        dbContext.AverageWages.Add(new AverageWageEntity
        {
            Year = averageWage.Year,
            GrossValue = averageWage.GrossValue,
            NetValue = averageWage.NetValue
        });
    }

    public void Delete(AverageWage averageWage)
    {
        ArgumentNullException.ThrowIfNull(averageWage);

        AverageWageEntity entity = dbContext.AverageWages.Find(averageWage.Year);

        if (entity != null)
            dbContext.AverageWages.Remove(entity);
    }

    private static AverageWage MapToDomain(AverageWageEntity entity)
    {
        return new AverageWage
        {
            Year = entity.Year,
            GrossValue = entity.GrossValue,
            NetValue = entity.NetValue
        };
    }
}