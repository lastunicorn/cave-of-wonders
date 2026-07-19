namespace DustInTheWind.CaveOfWonders.DbMigration;

internal readonly record struct DatabaseConfig(DatabaseType Type, string ConnectionString)
{
    public static DatabaseConfig Create(DatabaseType type, string connectionString)
    {
        string resolvedConnectionString = new CaveOfWondersConnectionString(connectionString);

        return new DatabaseConfig(type, resolvedConnectionString);
    }

    public bool IsSameDatabaseAs(DatabaseConfig other)
    {
        return Type == other.Type
            && string.Equals(ConnectionString, other.ConnectionString, StringComparison.OrdinalIgnoreCase);
    }
}
