using Microsoft.Extensions.Configuration;

namespace DustInTheWind.CaveOfWonders.DbMigration;

internal readonly record struct DatabaseConfig(string DatabaseType, string ConnectionString)
{
    public static DatabaseConfig Read(IConfigurationSection section)
    {
        string databaseType = section.GetValue<string>("DatabaseType")
            ?? throw new InvalidOperationException($"Missing 'DatabaseType' in configuration section '{section.Path}'.");

        string rawConnectionString = section.GetSection("ConnectionStrings").GetValue<string>(databaseType)
            ?? throw new InvalidOperationException($"Missing connection string for database type '{databaseType}' in configuration section '{section.Path}:ConnectionStrings'.");

        string connectionString = new CaveOfWondersConnectionString(rawConnectionString);

        return new DatabaseConfig(databaseType, connectionString);
    }
}
