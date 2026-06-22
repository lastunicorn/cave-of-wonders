using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Adapters.DataAccess.Json.Infrastructure;

internal class DatabaseTest : DatabaseTest<Database>
{
    private readonly string dbFilePath = Path.Combine(Path.GetTempPath(), $"test-database-{Guid.NewGuid()}");

    public static DatabaseTest Create()
    {
        return new DatabaseTest();
    }

    protected override async Task<Database> OpenDatabaseAsync()
    {
        Database database = new(dbFilePath);
        await database.LoadAsync(CancellationToken.None);
        return database;
    }

    protected override async Task CloseDatabaseAsync(Database database)
    {
        await database.SaveAsync(CancellationToken.None);
    }
    
    protected override void ResetDatabase()
    {
        if (Directory.Exists(dbFilePath))
            Directory.Delete(dbFilePath, true);
    }
}
