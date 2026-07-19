using System.Data.Common;

namespace DustInTheWind.CaveOfWonders.DbMigration;

internal static class ConnectionStringDataSource
{
    public static string Get(string connectionString)
    {
        DbConnectionStringBuilder connectionStringBuilder = new()
        {
            ConnectionString = connectionString
        };

        string dataSource = connectionStringBuilder.TryGetValue("Data Source", out object value)
            ? value as string
            : null;

        return dataSource ?? throw new ArgumentException("Connection string does not contain a 'Data Source'.", nameof(connectionString));
    }
}
