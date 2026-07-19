using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.CompiledModels;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using JsonDatabase = DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.Database;
using JsonUnitOfWork = DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.UnitOfWork;
using LiteDbContext = DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb.DbContext;
using LiteDbUnitOfWork = DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb.UnitOfWork;
using SqliteUnitOfWork = DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.UnitOfWork;

namespace DustInTheWind.CaveOfWonders.DbMigration;

/// <summary>
/// Wraps an <see cref="IUnitOfWork"/> together with whatever needs disposing
/// (an EF Core / LiteDB connection), and knows how to wipe the underlying
/// storage before it is used, regardless of which adapter is behind it.
/// </summary>
internal sealed class DatabaseEndpoint : IDisposable
{
    private readonly IDisposable disposable;

    public IUnitOfWork UnitOfWork { get; }

    private DatabaseEndpoint(IUnitOfWork unitOfWork, IDisposable disposable)
    {
        UnitOfWork = unitOfWork;
        this.disposable = disposable;
    }

    public static DatabaseEndpoint Create(DatabaseConfig config, bool cleanBeforeUse)
    {
        return config.DatabaseType.ToLowerInvariant() switch
        {
            "json" => CreateJson(config.ConnectionString, cleanBeforeUse),
            "sqlite" => CreateSqlite(config.ConnectionString, cleanBeforeUse),
            "litedb" => CreateLiteDb(config.ConnectionString, cleanBeforeUse),
            _ => throw new InvalidOperationException($"Unknown database type '{config.DatabaseType}'.")
        };
    }

    private static DatabaseEndpoint CreateJson(string connectionString, bool cleanBeforeUse)
    {
        if (cleanBeforeUse)
        {
            string databaseDirectoryPath = ConnectionStringDataSource.Get(connectionString);

            if (Directory.Exists(databaseDirectoryPath))
                Directory.Delete(databaseDirectoryPath, recursive: true);
        }

        JsonDatabase database = new(connectionString);
        JsonUnitOfWork unitOfWork = new(database);

        return new DatabaseEndpoint(unitOfWork, disposable: null);
    }

    private static DatabaseEndpoint CreateSqlite(string connectionString, bool cleanBeforeUse)
    {
        string dataSource = new SqliteConnectionStringBuilder(connectionString).DataSource;
        string databaseDirectoryPath = Path.GetDirectoryName(Path.GetFullPath(dataSource, AppContext.BaseDirectory));

        if (!string.IsNullOrEmpty(databaseDirectoryPath) && !Directory.Exists(databaseDirectoryPath))
            Directory.CreateDirectory(databaseDirectoryPath);

        DbContextOptionsBuilder<CaveOfWondersDbContext> optionsBuilder = new();
        optionsBuilder
            .UseSqlite(connectionString)
            .UseModel(CaveOfWondersDbContextModel.Instance);

        CaveOfWondersDbContext dbContext = new(optionsBuilder.Options);

        if (cleanBeforeUse)
            dbContext.Database.EnsureDeleted();

        dbContext.Database.Migrate();

        SqliteUnitOfWork unitOfWork = new(dbContext);

        return new DatabaseEndpoint(unitOfWork, dbContext);
    }

    private static DatabaseEndpoint CreateLiteDb(string connectionString, bool cleanBeforeUse)
    {
        if (cleanBeforeUse)
        {
            string filePath = ConnectionStringDataSource.Get(connectionString);
            string fullFilePath = Path.GetFullPath(filePath, AppContext.BaseDirectory);

            if (File.Exists(fullFilePath))
                File.Delete(fullFilePath);
        }

        LiteDbContext dbContext = new(connectionString);
        LiteDbUnitOfWork unitOfWork = new(dbContext);

        return new DatabaseEndpoint(unitOfWork, dbContext);
    }

    public void Dispose()
    {
        disposable?.Dispose();
    }
}
