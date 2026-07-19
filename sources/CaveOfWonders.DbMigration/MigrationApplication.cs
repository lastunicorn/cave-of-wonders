using DustInTheWind.CaveOfWonders.DbMigration.DatabaseEndpoints;
using Microsoft.Extensions.Logging;

namespace DustInTheWind.CaveOfWonders.DbMigration;

/// <summary>
/// Orchestrates one migration run: validates the source/destination pair, asks for
/// confirmation when the destination is about to be cleaned, opens both endpoints
/// and executes the <see cref="Migration"/>. Returns the process exit code.
/// </summary>
internal sealed class MigrationApplication
{
    private readonly MigrationOptions options;
    private readonly IDatabaseEndpointFactory databaseEndpointFactory;
    private readonly Migration migration;
    private readonly ILogger<MigrationApplication> logger;

    public MigrationApplication(
        MigrationOptions options,
        IDatabaseEndpointFactory databaseEndpointFactory,
        Migration migration,
        ILogger<MigrationApplication> logger)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.databaseEndpointFactory = databaseEndpointFactory ?? throw new ArgumentNullException(nameof(databaseEndpointFactory));
        this.migration = migration ?? throw new ArgumentNullException(nameof(migration));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<int> RunAsync(CancellationToken cancellationToken)
    {
        DatabaseConfig sourceConfig = DatabaseConfig.Create(options.SourceType, options.Source);
        DatabaseConfig destinationConfig = DatabaseConfig.Create(options.DestinationType, options.Destination);

        if (sourceConfig.IsSameDatabaseAs(destinationConfig))
        {
            logger.LogError("Source and destination resolve to the same database. Aborting.");
            return 1;
        }

        // The summary and the confirmation prompt are written directly to the console,
        // not through ILogger: the console logger flushes on a background thread, so
        // logged lines would interleave with the synchronous prompt text.
        Console.WriteLine($"Source:      {sourceConfig.Type}");
        Console.WriteLine($"Destination: {destinationConfig.Type}{(options.CleanDestination ? " (will be cleaned before migrating)" : "")}");
        Console.WriteLine();

        if (options.CleanDestination && !options.SkipConfirmation && !ConfirmDestructiveMigration())
        {
            logger.LogInformation("Migration cancelled.");
            return 1;
        }

        try
        {
            using IDatabaseEndpoint source = databaseEndpointFactory.Create(sourceConfig, cleanBeforeUse: false);
            using IDatabaseEndpoint destination = databaseEndpointFactory.Create(destinationConfig, cleanBeforeUse: options.CleanDestination);

            await migration.RunAsync(source.UnitOfWork, destination.UnitOfWork, cancellationToken);

            logger.LogInformation("");
            logger.LogInformation("Migration completed successfully.");

            return 0;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation("");
            logger.LogError("Migration cancelled.");
            return 1;
        }
        catch (Exception ex)
        {
            logger.LogInformation("");
            logger.LogError("Migration failed: {ErrorMessage}", ex.Message);
            return 1;
        }
    }

    private static bool ConfirmDestructiveMigration()
    {
        Console.WriteLine("WARNING: this will permanently delete all existing data in the destination database.");
        Console.Write("Type 'yes' to continue: ");
        string answer = Console.ReadLine();

        return string.Equals(answer, "yes", StringComparison.OrdinalIgnoreCase);
    }
}
