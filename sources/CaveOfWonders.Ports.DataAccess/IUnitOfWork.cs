namespace DustInTheWind.CaveOfWonders.Ports.DataAccess;

public interface IUnitOfWork
{
    public IPotRepository PotRepository { get; }

    public IExchangeRateRepository ExchangeRateRepository { get; }

    public IInflationRecordRepository InflationRecordRepository { get; }

    public Task SaveChanges();
}