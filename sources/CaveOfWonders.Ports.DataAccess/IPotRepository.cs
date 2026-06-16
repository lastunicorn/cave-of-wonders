using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Ports.DataAccess;

public interface IPotRepository
{
    Task<IEnumerable<Pot>> GetAllAsync(CancellationToken cancellationToken = default);
    
    Task<Pot> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<PotSnapshot>> GetSnapshots(DateOnly date, DateMatchingMode dateMatchingMode, bool includeInactive);

    IAsyncEnumerable<Pot> GetByPartialIdAsync(string partialPotId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Pot>> GetByIdOrName(string idOrName, CancellationToken cancellationToken = default);
    
    void Add(Pot pot);
}