using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Repositories;

internal class PotSnapshotRepository : IPotSnapshotRepository
{
	private readonly CaveOfWondersDbContext dbContext;

	public PotSnapshotRepository(CaveOfWondersDbContext dbContext)
	{
		this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
	}

	public async Task<IEnumerable<PotSnapshot>> GetSnapshotsAsync(DateOnly date, DateMatchingMode dateMatchingMode, bool includeInactive, CancellationToken cancellationToken = default)
	{
		List<Pot> pots = await dbContext.Pots
			.Include(x => x.Snapshots)
			.ToListAsync(cancellationToken);

		return pots
			.Where(x => includeInactive || x.IsActive(date))
			.Select(x => GetLatestSnapshot(x, date, dateMatchingMode))
			.Where(x => x != null);
	}

	private static PotSnapshot GetLatestSnapshot(Pot pot, DateOnly date, DateMatchingMode dateMatchingMode)
	{
		return dateMatchingMode switch
		{
			DateMatchingMode.Exact => pot.Snapshots
				.FirstOrDefault(x => x.Date == date),
			DateMatchingMode.LastAvailable => pot.Snapshots
				.Where(x => x.Date <= date)
				.MaxBy(x => x.Date),
			_ => throw new ArgumentOutOfRangeException(nameof(dateMatchingMode))
		};
	}
}
