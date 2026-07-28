namespace DustInTheWind.CaveOfWonders.Ports.DataAccess;

public class DatabaseConfiguration : IDatabaseConfiguration
{
    public string ConnectionString { get; }

    public DatabaseConfiguration(string connectionString)
    {
        ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }
}
