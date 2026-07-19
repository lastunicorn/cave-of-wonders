using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;

namespace DustInTheWind.CaveOfWonders.DbMigration.DatabaseEndpoints;

internal sealed class LiteDbDatabaseEndpoint : IDatabaseEndpoint
{
    private readonly DbContext dbContext;

    public IUnitOfWork UnitOfWork { get; }

    public LiteDbDatabaseEndpoint(string connectionString, bool cleanBeforeUse)
    {
        if (cleanBeforeUse)
        {
            string filePath = ConnectionStringDataSource.Get(connectionString);
            string fullFilePath = Path.GetFullPath(filePath, AppContext.BaseDirectory);

            if (File.Exists(fullFilePath))
                File.Delete(fullFilePath);
        }

        dbContext = new DbContext(connectionString);
        UnitOfWork = new UnitOfWork(dbContext);
    }

    public void Dispose()
    {
        dbContext.Dispose();
    }
}
