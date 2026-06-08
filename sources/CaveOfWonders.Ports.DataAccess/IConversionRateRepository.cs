using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Ports.DataAccess;

public interface IConversionRateRepository
{
    Task<IEnumerable<ExchangeRate>> GetAll(DateTime date);
}