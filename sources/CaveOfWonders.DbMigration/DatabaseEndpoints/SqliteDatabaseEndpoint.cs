using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.CompiledModels;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DustInTheWind.CaveOfWonders.DbMigration.DatabaseEndpoints;

internal sealed class SqliteDatabaseEndpoint : IDatabaseEndpoint
{
    private readonly CaveOfWondersDbContext dbContext;

    public IUnitOfWork UnitOfWork { get; }

    public SqliteDatabaseEndpoint(string connectionString, bool cleanBeforeUse)
    {
        string dataSource = new SqliteConnectionStringBuilder(connectionString).DataSource;
        string databaseDirectoryPath = Path.GetDirectoryName(Path.GetFullPath(dataSource, AppContext.BaseDirectory));

        if (!string.IsNullOrEmpty(databaseDirectoryPath) && !Directory.Exists(databaseDirectoryPath))
            Directory.CreateDirectory(databaseDirectoryPath);

        DbContextOptionsBuilder<CaveOfWondersDbContext> optionsBuilder = new();
        optionsBuilder
            .UseSqlite(connectionString)
            .UseModel(CaveOfWondersDbContextModel.Instance);

        dbContext = new CaveOfWondersDbContext(optionsBuilder.Options);

        if (cleanBeforeUse)
            dbContext.Database.EnsureDeleted();

        dbContext.Database.Migrate();

        UnitOfWork = new UnitOfWork(dbContext);
    }

    public void Dispose()
    {
        dbContext.Dispose();
    }
}
