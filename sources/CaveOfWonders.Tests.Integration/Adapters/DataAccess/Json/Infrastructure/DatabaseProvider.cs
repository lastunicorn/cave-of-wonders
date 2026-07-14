using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Adapters.DataAccess.Json.Infrastructure;

internal class DatabaseProvider : ISutProvider<Database>
{
    private readonly string dbDirectoryPath = Path.Combine(Path.GetTempPath(), $"test-database-{Guid.NewGuid()}");

    public async Task<Database> CreateAsync(CancellationToken cancellationToken = default)
    {
        Database database = new(dbDirectoryPath);
        await database.LoadAsync(cancellationToken);
        return database;
    }

    public Task ReleaseAsync(Database database, CancellationToken cancellationToken = default)
    {
        return database.SaveAsync(cancellationToken);
    }

    public void Reset()
    {
        if (Directory.Exists(dbDirectoryPath))
            Directory.Delete(dbDirectoryPath, true);
    }
}
