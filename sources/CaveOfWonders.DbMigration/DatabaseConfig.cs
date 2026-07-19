namespace DustInTheWind.CaveOfWonders.DbMigration;

internal readonly record struct DatabaseConfig(string DatabaseType, string ConnectionString)
{
    public static DatabaseConfig FromArguments(string databaseType, string connectionString)
    {
        string resolvedConnectionString = new CaveOfWondersConnectionString(connectionString);

        return new DatabaseConfig(databaseType, resolvedConnectionString);
    }
}
