using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.Repositories;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;

public class UnitOfWork : IUnitOfWork
{
    private readonly Database database;

    private IPotRepository potRepository;
    private IExchangeRateRepository exchangeRateRepository;
    private ICpiRepository cpiRepository;
    private IAverageWageRepository averageWageRepository;
    private IGemRepository gemRepository;

    public IPotRepository PotRepository => potRepository ??= new PotRepository(database);

    public IExchangeRateRepository ExchangeRateRepository => exchangeRateRepository ??= new ExchangeRateRepository(database);

    public ICpiRepository CpiRepository => cpiRepository ??= new CpiRepository(database);

    public IAverageWageRepository AverageWageRepository => averageWageRepository ??= new AverageWageRepository(database);

    public IGemRepository GemRepository => gemRepository ??= new GemRepository(database);

    public UnitOfWork(Database database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database));

        database.LoadAsync(CancellationToken.None).Wait();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return database.SaveAsync(cancellationToken);
    }
}