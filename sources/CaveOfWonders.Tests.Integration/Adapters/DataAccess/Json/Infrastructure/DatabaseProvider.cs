using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Adapters.DataAccess.Json.Infrastructure;

internal class DatabaseProvider : ISutProvider<Database>
{
    private readonly string dbDirectoryPath = Path.Combine(Path.GetTempPath(), $"test-database-{Guid.NewGuid()}");

    public async Task<Database> CreateAsync()
    {
        Database database = new(dbDirectoryPath);
        await database.LoadAsync(CancellationToken.None);
        return database;
    }

    public Task ReleaseAsync(Database database)
    {
        return database.SaveAsync(CancellationToken.None);
    }

    public void Reset()
    {
        if (Directory.Exists(dbDirectoryPath))
            Directory.Delete(dbDirectoryPath, true);
    }
}
