namespace DustInTheWind.CaveOfWonders.Ports.DataAccess;

public interface IUnitOfWork
{
    public IPotRepository PotRepository { get; }

    public IExchangeRateRepository ExchangeRateRepository { get; }

    public ICpiRepository CpiRepository { get; }

    IAverageWageRepository AverageWageRepository { get; }

    IGemRepository GemRepository { get; }

    public Task SaveChanges();
}