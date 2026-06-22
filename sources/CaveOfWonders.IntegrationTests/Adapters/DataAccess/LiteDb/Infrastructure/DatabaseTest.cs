using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.IntegrationTests.Adapters.DataAccess.LiteDb.Infrastructure;

internal class DatabaseTest : DatabaseTest<DbContext>
{
    public static DatabaseTest Create()
    {
        return new DatabaseTest();
    }

    private DatabaseTest()
        : base(Path.Combine(Path.GetTempPath(), $"test-dbContext-{Guid.NewGuid()}"))
    {
    }

    protected override Task<DbContext> OpenDatabaseAsync()
    {
        return Task.FromResult(new DbContext(DbPath));
    }

    protected override Task CloseDatabaseAsync(DbContext dbContext)
    {
        dbContext.Dispose();
        return Task.CompletedTask;
    }
}
