using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.Providers;

internal class LiteDbPotRepositoryProvider : ISutProvider<IPotRepository>
{
    private readonly string dbFilePath = Path.Combine(Path.GetTempPath(), $"test-dbContext-{Guid.NewGuid()}");

    private DbContext dbContext;

    public Task<IPotRepository> CreateAsync(CancellationToken cancellationToken = default)
    {
        dbContext = new DbContext(dbFilePath);
        IPotRepository repository = new PotRepository(dbContext);
        return Task.FromResult(repository);
    }

    public Task ReleaseAsync(IPotRepository repository, CancellationToken cancellationToken = default)
    {
        dbContext.Dispose();
        return Task.CompletedTask;
    }

    public void Reset()
    {
        if (File.Exists(dbFilePath))
            File.Delete(dbFilePath);
    }

    public override string ToString()
    {
        return "LiteDb";
    }
}
