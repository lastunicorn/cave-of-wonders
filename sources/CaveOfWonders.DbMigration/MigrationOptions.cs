namespace DustInTheWind.CaveOfWonders.DbMigration;

internal sealed record MigrationOptions(
    string Source,
    DatabaseType SourceType,
    string Destination,
    DatabaseType DestinationType,
    bool CleanDestination,
    bool SkipConfirmation);
