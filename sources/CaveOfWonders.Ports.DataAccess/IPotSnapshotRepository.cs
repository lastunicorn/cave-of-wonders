using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Ports.DataAccess;

public interface IPotSnapshotRepository
{
	Task<IEnumerable<PotSnapshot>> GetLatestAsync(DateOnly date, DateMatchingMode dateMatchingMode, bool includeInactive, CancellationToken cancellationToken = default);
}
