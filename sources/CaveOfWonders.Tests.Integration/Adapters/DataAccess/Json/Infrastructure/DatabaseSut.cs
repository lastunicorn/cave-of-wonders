using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Adapters.DataAccess.Json.Infrastructure;

internal class DatabaseSut : ISut<Database>
{
    private readonly string dbDirectoryPath = Path.Combine(Path.GetTempPath(), $"test-database-{Guid.NewGuid()}");

    public Database Instance { get; private set; }

    public async Task CreateInstanceAsync(CancellationToken cancellationToken = default)
    {
        Instance = new Database(dbDirectoryPath);
        await Instance.LoadAsync(cancellationToken);
    }

    public async Task ReleaseInstanceAsync(CancellationToken cancellationToken = default)
    {
        await Instance.SaveAsync(cancellationToken);
        Instance = null;
    }

    public Task ResetAsync(CancellationToken cancellationToken = default)
    {
        if (Directory.Exists(dbDirectoryPath))
            Directory.Delete(dbDirectoryPath, true);

        return Task.CompletedTask;
    }
}
