using System.CommandLine;
using System.CommandLine.Parsing;

namespace DustInTheWind.CaveOfWonders.DbMigration;

/// <summary>
/// Declares the command-line surface of the tool and binds parsed arguments
/// into a <see cref="MigrationOptions"/> before handing off to the host.
/// </summary>
internal static class MigrateCommand
{
    public static RootCommand Create()
    {
        Option<string> sourceOption = new("--source", "-s")
        {
            Description = "Connection string for the source database.",
            HelpName = "connection-string",
            Required = true
        };

        Option<DatabaseType> sourceTypeOption = new("--source-type")
        {
            Description = "Type of the source database (json, sqlite, litedb).",
            HelpName = "type",
            Required = true,
            CustomParser = ParseDatabaseType
        };

        Option<string> destinationOption = new("--destination", "-d")
        {
            Description = "Connection string for the destination database.",
            HelpName = "connection-string",
            Required = true
        };

        Option<DatabaseType> destinationTypeOption = new("--destination-type")
        {
            Description = "Type of the destination database (json, sqlite, litedb).",
            HelpName = "type",
            Required = true,
            CustomParser = ParseDatabaseType
        };

        Option<bool> cleanDestinationOption = new("--clean-destination", "--clean")
        {
            Description = "Delete all existing data in the destination database before migrating."
        };

        Option<bool> skipConfirmationOption = new("--yes", "-y")
        {
            Description = "Do not prompt for confirmation when --clean-destination is used."
        };

        RootCommand rootCommand = new(
            "Copies all data (pots, exchange rates, CPI, average wages, gems) from the " +
            "source database into the destination database.")
        {
            sourceOption,
            sourceTypeOption,
            destinationOption,
            destinationTypeOption,
            cleanDestinationOption,
            skipConfirmationOption
        };

        rootCommand.SetAction((parseResult, cancellationToken) =>
        {
            MigrationOptions options = new(
                Source: parseResult.GetValue(sourceOption),
                SourceType: parseResult.GetValue(sourceTypeOption),
                Destination: parseResult.GetValue(destinationOption),
                DestinationType: parseResult.GetValue(destinationTypeOption),
                CleanDestination: parseResult.GetValue(cleanDestinationOption),
                SkipConfirmation: parseResult.GetValue(skipConfirmationOption));

            return MigrationHost.RunAsync(options, cancellationToken);
        });

        return rootCommand;
    }

    private static DatabaseType ParseDatabaseType(ArgumentResult argumentResult)
    {
        string token = argumentResult.Tokens[^1].Value;

        if (Enum.TryParse(token, ignoreCase: true, out DatabaseType databaseType))
            return databaseType;

        argumentResult.AddError($"Unknown database type '{token}'. Valid values: json, sqlite, litedb.");
        return default;
    }
}
