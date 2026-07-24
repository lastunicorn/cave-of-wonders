using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Ports.DataAccess;

public interface IPotSnapshotRepository
{
	Task<IEnumerable<PotSnapshot>> GetLatestAsync(DateOnly date, DateMatchingMode dateMatchingMode, bool includeInactive, CancellationToken cancellationToken = default);

	IAsyncEnumerable<PotSnapshot> GetByPotIdAsync(Guid potId, DateOnly? startDate = null, DateOnly? endDate = null, CancellationToken cancellationToken = default);

	Task<int> GetCountAsync(Guid potId, CancellationToken cancellationToken = default);

	Task<PotSnapshot> GetLatestByPotIdAsync(Guid potId, CancellationToken cancellationToken = default);

	void Add(PotSnapshot potSnapshot);

	void AddRange(IEnumerable<PotSnapshot> potSnapshots);

	void RemoveByPotId(Guid potId);
}
