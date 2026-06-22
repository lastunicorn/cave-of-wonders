using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Entities;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Repositories;

internal class CpiRepository : ICpiRepository
{
    private readonly CaveOfWondersDbContext dbContext;

    public CpiRepository(CaveOfWondersDbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IEnumerable<Cpi>> GetAll()
    {
        List<CpiEntity> entities = await dbContext.Cpis
            .OrderBy(x => x.Year)
            .ToListAsync();

        return entities.Select(MapToDomain);
    }

    public async Task<Cpi> GetByYear(int year)
    {
        CpiEntity entity = await dbContext.Cpis.FindAsync(year);

        return entity == null ? null : MapToDomain(entity);
    }

    public void Add(Cpi cpi)
    {
        ArgumentNullException.ThrowIfNull(cpi);

        dbContext.Cpis.Add(new CpiEntity
        {
            Year = cpi.Year,
            Value = cpi.Value
        });
    }

    private static Cpi MapToDomain(CpiEntity entity)
    {
        return new Cpi
        {
            Year = entity.Year,
            Value = entity.Value
        };
    }
}
