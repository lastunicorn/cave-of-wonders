using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;

namespace DustInTheWind.CaveOfWonders.DbMigration.DatabaseEndpoints;

internal sealed class JsonDatabaseEndpoint : IDatabaseEndpoint
{
    public IUnitOfWork UnitOfWork { get; }

    public JsonDatabaseEndpoint(string connectionString, bool cleanBeforeUse)
    {
        if (cleanBeforeUse)
        {
            string databaseDirectoryPath = ConnectionStringDataSource.Get(connectionString);

            if (Directory.Exists(databaseDirectoryPath))
                Directory.Delete(databaseDirectoryPath, recursive: true);
        }

        Database database = new(connectionString);
        UnitOfWork = new UnitOfWork(database);
    }

    public void Dispose()
    {
    }
}
