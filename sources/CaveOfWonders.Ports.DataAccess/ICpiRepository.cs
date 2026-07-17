using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Ports.DataAccess;

public interface ICpiRepository
{
    IAsyncEnumerable<Cpi> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Cpi> GetByYear(int year, CancellationToken cancellationToken = default);

    void Add(Cpi cpi);
}