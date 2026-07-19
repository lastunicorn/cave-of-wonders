using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Ports.DataAccess;

public interface IPotRepository
{
    IAsyncEnumerable<Pot> GetAllAsync(CancellationToken cancellationToken = default);
    
    Task<Pot> GetAsync(Guid id, CancellationToken cancellationToken = default);

    IAsyncEnumerable<Pot> GetAsync(PotFlexId potFlexId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<PotSnapshot>> GetSnapshotsAsync(DateOnly date, DateMatchingMode dateMatchingMode, bool includeInactive, CancellationToken cancellationToken = default);

    void Add(Pot pot);

    void Remove(Pot pot);
}