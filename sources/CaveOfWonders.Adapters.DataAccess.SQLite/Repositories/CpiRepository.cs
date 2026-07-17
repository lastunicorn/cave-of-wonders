using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Entities;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Mappers;
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

	public IAsyncEnumerable<Cpi> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return dbContext.Cpis
			.OrderBy(x => x.Year)
			.Select(x => new Cpi
			{
				Year = x.Year,
				Value = x.Value
			})
			.AsAsyncEnumerable();
	}

	public async Task<Cpi> GetByYear(int year, CancellationToken cancellationToken = default)
	{
		CpiEntity entity = await dbContext.Cpis.FindAsync([year], cancellationToken);
		return entity?.ToDomain();
	}

	public void Add(Cpi cpi)
	{
		ArgumentNullException.ThrowIfNull(cpi);

		CpiEntity cpiEntity = cpi.ToEntity();
		dbContext.Cpis.Add(cpiEntity);
	}
}