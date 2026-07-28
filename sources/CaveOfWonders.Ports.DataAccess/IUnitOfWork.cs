namespace DustInTheWind.CaveOfWonders.Ports.DataAccess;

public interface IUnitOfWork
{
	IPotRepository PotRepository { get; }

	IPotSnapshotRepository PotSnapshotRepository { get; }

	IExchangeRateRepository ExchangeRateRepository { get; }

	ICpiRepository CpiRepository { get; }

	IAverageWageRepository AverageWageRepository { get; }

	IGemRepository GemRepository { get; }

	Task SaveChangesAsync(CancellationToken cancellationToken = default);
}