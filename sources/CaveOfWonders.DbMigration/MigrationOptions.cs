namespace DustInTheWind.CaveOfWonders.DbMigration;

internal sealed class MigrationOptions
{
    public bool CleanDestination { get; private init; }

    public bool SkipConfirmation { get; private init; }

    public bool ShowHelp { get; private init; }

    public static MigrationOptions Parse(string[] args)
    {
        bool cleanDestination = false;
        bool skipConfirmation = false;
        bool showHelp = false;

        foreach (string arg in args)
        {
            switch (arg)
            {
                case "--clean-destination":
                case "--clean":
                    cleanDestination = true;
                    break;

                case "--yes":
                case "-y":
                    skipConfirmation = true;
                    break;

                case "--help":
                case "-h":
                    showHelp = true;
                    break;

                default:
                    throw new ArgumentException($"Unknown argument '{arg}'.");
            }
        }

        return new MigrationOptions
        {
            CleanDestination = cleanDestination,
            SkipConfirmation = skipConfirmation,
            ShowHelp = showHelp
        };
    }

    public static void PrintUsage()
    {
        Console.WriteLine("""
            CaveOfWonders.DbMigration

            Copies all data (pots, exchange rates, CPI, average wages, gems) from the
            "Source" database into the "Destination" database, as configured in
            appsettings.json.

            Usage:
              dotnet CaveOfWonders.DbMigration.dll [options]

            Options:
              --clean-destination, --clean   Delete all existing data in the destination
                                              database before migrating.
              --yes, -y                      Do not prompt for confirmation when
                                              --clean-destination is used.
              --help, -h                     Show this help message.
            """);
    }
}
