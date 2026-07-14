using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Adapters.DataAccess.LiteDb.Infrastructure;

internal class DatabaseProvider : ISutProvider<DbContext>
{
    private readonly string dbFilePath = Path.Combine(Path.GetTempPath(), $"test-dbContext-{Guid.NewGuid()}");

    public Task<DbContext> CreateAsync()
    {
        DbContext dbContext = new(dbFilePath);
        return Task.FromResult(dbContext);
    }

    public Task ReleaseAsync(DbContext dbContext)
    {
        dbContext.Dispose();
        return Task.CompletedTask;
    }

    public void Reset()
    {
        if (File.Exists(dbFilePath))
            File.Delete(dbFilePath);
    }
}
