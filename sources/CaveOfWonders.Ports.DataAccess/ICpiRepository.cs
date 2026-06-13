using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Ports.DataAccess;

public interface ICpiRepository
{
    Task<IEnumerable<Cpi>> GetAll();

    Task Add(Cpi cpiDto);

    Task<AddOrUpdateResult> AddOrUpdate(Cpi cpiDto);
}