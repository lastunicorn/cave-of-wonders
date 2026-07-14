using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Adapters.DataAccess.LiteDb.Infrastructure;

internal class DatabaseSut : ISut<DbContext>
{
    private readonly string dbFilePath = Path.Combine(Path.GetTempPath(), $"test-dbContext-{Guid.NewGuid()}");

    public DbContext Instance { get; private set; }

    public Task CreateInstanceAsync(CancellationToken cancellationToken = default)
    {
        Instance = new DbContext(dbFilePath);
        
        return Task.CompletedTask;
    }

    public Task ReleaseInstanceAsync(CancellationToken cancellationToken = default)
    {
        Instance.Dispose();
        Instance = null;
        
        return Task.CompletedTask;
    }

    public void Reset()
    {
        if (File.Exists(dbFilePath))
            File.Delete(dbFilePath);
    }
}
