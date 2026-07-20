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
			.AsAsyncEnumerable();
	}

	public async Task<Cpi> GetByYear(int year, CancellationToken cancellationToken = default)
	{
		return await dbContext.Cpis.FindAsync([year], cancellationToken);
	}

	public void Add(Cpi cpi)
	{
		ArgumentNullException.ThrowIfNull(cpi);

		dbContext.Cpis.Add(cpi);
	}
}
