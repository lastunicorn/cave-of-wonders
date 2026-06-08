using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;

namespace DustInTheWind.CaveOfWonders.IntegrationTests.Adapters.DataAccess.LiteDb;

public abstract class DatabaseTests : IDisposable
{
    private readonly string dbFilePath;

    protected DbContext DbContext { get; }

    public DatabaseTests()
    {
        dbFilePath = Path.Combine(Path.GetTempPath(), $"test-database-{Guid.NewGuid()}.db");
        DbContext = new DbContext(dbFilePath);
    }

    public void Dispose()
    {
        Dispose(true);
    }

    protected virtual void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            DbContext?.Dispose();

            if (File.Exists(dbFilePath))
                File.Delete(dbFilePath);
        }
    }
}