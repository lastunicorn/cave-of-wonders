namespace DustInTheWind.CaveOfWonders.DbMigration.DatabaseEndpoints;

internal interface IDatabaseEndpointFactory
{
    IDatabaseEndpoint Create(DatabaseConfig databaseConfig, bool cleanBeforeUse);
}

internal sealed class DatabaseEndpointFactory : IDatabaseEndpointFactory
{
    public IDatabaseEndpoint Create(DatabaseConfig databaseConfig, bool cleanBeforeUse)
    {
        return databaseConfig.Type switch
        {
            DatabaseType.Json => new JsonDatabaseEndpoint(databaseConfig.ConnectionString, cleanBeforeUse),
            DatabaseType.Sqlite => new SqliteDatabaseEndpoint(databaseConfig.ConnectionString, cleanBeforeUse),
            DatabaseType.LiteDb => new LiteDbDatabaseEndpoint(databaseConfig.ConnectionString, cleanBeforeUse),
            _ => throw new InvalidOperationException($"Unknown database type '{databaseConfig.Type}'.")
        };
    }
}
