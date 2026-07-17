using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Ports.DataAccess;

public interface IAverageWageRepository
{
    IAsyncEnumerable<AverageWage> GetAllAsync(CancellationToken cancellationToken = default);

    Task<AverageWage> GetAsync(int year, CancellationToken cancellationToken);

    void Add(AverageWage averageWage);

    void Delete(AverageWage averageWage);
}