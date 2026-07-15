using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Ports.DataAccess;

public interface ICpiRepository
{
    IAsyncEnumerable<Cpi> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Cpi> GetByYear(int year);

    void Add(Cpi cpi);
}