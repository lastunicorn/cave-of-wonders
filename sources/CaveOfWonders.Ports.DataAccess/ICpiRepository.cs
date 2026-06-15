using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Ports.DataAccess;

public interface ICpiRepository
{
    Task<IEnumerable<Cpi>> GetAll();

    Task<Cpi> GetByYear(int year);

    void Add(Cpi cpi);
}