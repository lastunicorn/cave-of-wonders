using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.Providers;

internal class JsonPotRepositoryProvider : ISutProvider<IPotRepository>
{
    private readonly string dbDirectoryPath = Path.Combine(Path.GetTempPath(), $"test-database-{Guid.NewGuid()}");

    private Database database;

    public async Task<IPotRepository> CreateAsync()
    {
        database = new Database(dbDirectoryPath);
        await database.LoadAsync(CancellationToken.None);
        return new PotRepository(database);
    }

    public Task ReleaseAsync(IPotRepository repository)
    {
        return database.SaveAsync(CancellationToken.None);
    }

    public void Reset()
    {
        if (Directory.Exists(dbDirectoryPath))
            Directory.Delete(dbDirectoryPath, true);
    }

    public override string ToString()
    {
        return "Json";
    }
}
