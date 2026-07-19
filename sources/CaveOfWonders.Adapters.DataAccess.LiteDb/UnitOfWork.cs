using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;

public sealed class UnitOfWork : IUnitOfWork, IDisposable
{
	private readonly DbContext dbContext;

	private IPotRepository potRepository;
	private IExchangeRateRepository exchangeRateRepository;
	private IGemRepository gemRepository;

	public IPotRepository PotRepository => potRepository ??= new PotRepository(dbContext);

	public IExchangeRateRepository ExchangeRateRepository => exchangeRateRepository ??= new ExchangeRateRepository(dbContext);

	public ICpiRepository CpiRepository => null;

	public IAverageWageRepository AverageWageRepository => null;

	public IGemRepository GemRepository => gemRepository ??= new GemRepository(dbContext);

	public UnitOfWork(DbContext dbContext)
	{
		this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
	}

	public Task SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		dbContext.SaveChanges();
		return Task.CompletedTask;
	}

	public void Dispose()
	{
		dbContext?.Dispose();
	}
}