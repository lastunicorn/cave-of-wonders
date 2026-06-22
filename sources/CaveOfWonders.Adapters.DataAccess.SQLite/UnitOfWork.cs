using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Repositories;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite;

public sealed class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly CaveOfWondersDbContext dbContext;

    private PotRepository potRepository;
    private ExchangeRateRepository exchangeRateRepository;
    private CpiRepository cpiRepository;
    private AverageWageRepository averageWageRepository;
    private GemRepository gemRepository;

    public IPotRepository PotRepository => potRepository ??= new PotRepository(dbContext);

    public IExchangeRateRepository ExchangeRateRepository => exchangeRateRepository ??= new ExchangeRateRepository(dbContext);
    
    public ICpiRepository CpiRepository => cpiRepository ??= new CpiRepository(dbContext);
    
    public IAverageWageRepository AverageWageRepository => averageWageRepository ??= new AverageWageRepository(dbContext);
    
    public IGemRepository GemRepository => gemRepository ??= new GemRepository(dbContext);

    public UnitOfWork(CaveOfWondersDbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        dbContext?.Dispose();
    }
}
