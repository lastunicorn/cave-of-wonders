using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Ports.DataAccess;

public interface IPotRepository
{
    Task<IEnumerable<Pot>> GetAll();

    Task<IEnumerable<PotInstance>> GetInstances(DateTime date, DateMatchingMode dateMatchingMode, bool includeInactive);

    Task<IEnumerable<Pot>> GetByPartialId(string partialPotId);

    Task<IEnumerable<Pot>> GetByIdOrName(string idOrName);
    
    Task Add(Pot pot);
}