using DustInTheWind.CaveOfWonders.DbMigration;
using Microsoft.Extensions.Configuration;

MigrationOptions options;

try
{
    options = MigrationOptions.Parse(args);
}
catch (ArgumentException ex)
{
    Console.Error.WriteLine(ex.Message);
    MigrationOptions.PrintUsage();
    return 1;
}

if (options.ShowHelp)
{
    MigrationOptions.PrintUsage();
    return 0;
}

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .Build();

DatabaseConfig sourceConfig = DatabaseConfig.Read(configuration.GetSection("Source"));
DatabaseConfig destinationConfig = DatabaseConfig.Read(configuration.GetSection("Destination"));

bool sameDatabase = string.Equals(sourceConfig.DatabaseType, destinationConfig.DatabaseType, StringComparison.OrdinalIgnoreCase)
    && string.Equals(sourceConfig.ConnectionString, destinationConfig.ConnectionString, StringComparison.OrdinalIgnoreCase);

if (sameDatabase)
{
    Console.Error.WriteLine("Source and destination resolve to the same database. Aborting.");
    return 1;
}

Console.WriteLine($"Source:      {sourceConfig.DatabaseType}");
Console.WriteLine($"Destination: {destinationConfig.DatabaseType}{(options.CleanDestination ? " (will be cleaned before migrating)" : "")}");
Console.WriteLine();

if (options.CleanDestination && !options.SkipConfirmation)
{
    Console.WriteLine("WARNING: this will permanently delete all existing data in the destination database.");
    Console.Write("Type 'yes' to continue: ");
    string answer = Console.ReadLine();

    if (!string.Equals(answer, "yes", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Migration cancelled.");
        return 1;
    }
}

try
{
    using DatabaseEndpoint source = DatabaseEndpoint.Create(sourceConfig, cleanBeforeUse: false);
    using DatabaseEndpoint destination = DatabaseEndpoint.Create(destinationConfig, cleanBeforeUse: options.CleanDestination);

    Migrator migrator = new(source.UnitOfWork, destination.UnitOfWork);
    await migrator.RunAsync();

    Console.WriteLine();
    Console.WriteLine("Migration completed successfully.");

    return 0;
}
catch (Exception ex)
{
    Console.WriteLine();
    Console.Error.WriteLine($"Migration failed: {ex.Message}");
    return 1;
}
