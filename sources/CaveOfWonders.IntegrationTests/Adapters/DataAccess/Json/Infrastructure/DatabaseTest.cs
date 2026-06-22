using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.IntegrationTests.Adapters.DataAccess.Json.Infrastructure;

internal class DatabaseTest : DatabaseTest<Database>
{
    public static DatabaseTest Create()
    {
        return new DatabaseTest();
    }

    private DatabaseTest()
        : base(Path.Combine(Path.GetTempPath(), $"test-database-{Guid.NewGuid()}"))
    {
    }

    protected override async Task<Database> OpenDatabaseAsync()
    {
        Database database = new(DbPath);
        await database.LoadAsync(CancellationToken.None);
        return database;
    }

    protected override async Task CloseDatabaseAsync(Database database)
    {
        await database.SaveAsync(CancellationToken.None);
    }
}
