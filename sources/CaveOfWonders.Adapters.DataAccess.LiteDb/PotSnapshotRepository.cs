using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;

public class PotSnapshotRepository : IPotSnapshotRepository
{
	private readonly DbContext dbContext;

	public PotSnapshotRepository(DbContext dbContext)
	{
		this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
	}

	public Task<IEnumerable<PotSnapshot>> GetLatestAsync(DateOnly date, DateMatchingMode dateMatchingMode, bool includeInactive, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		IEnumerable<PotSnapshot> potInstances = dbContext.Pots
			.FindAll()
			.Select(x => x.ToDomainEntity())
			.Where(x => includeInactive || x.IsActive(date))
			.Select(x => x.GetSnapshot(date, dateMatchingMode))
			.Where(x => x != null);

		return Task.FromResult(potInstances);
	}
}
