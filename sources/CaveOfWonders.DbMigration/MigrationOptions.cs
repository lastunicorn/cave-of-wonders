namespace DustInTheWind.CaveOfWonders.DbMigration;

internal sealed class MigrationOptions
{
	public string Source { get; private init; }

	public string Destination { get; private init; }

	public string SourceType { get; private init; }

	public string DestinationType { get; private init; }

	public bool CleanDestination { get; private init; }

	public bool SkipConfirmation { get; private init; }

	public bool ShowHelp { get; private init; }

	public static MigrationOptions Parse(string[] args)
	{
		string source = null;
		string destination = null;
		string sourceType = null;
		string destinationType = null;
		bool cleanDestination = false;
		bool skipConfirmation = false;
		bool showHelp = false;

		for (int i = 0; i < args.Length; i++)
		{
			string arg = args[i];

			switch (arg)
			{
				case "--source":
				case "-s":
					source = GetValue(args, ref i, arg);
					break;

				case "--destination":
				case "-d":
					destination = GetValue(args, ref i, arg);
					break;

				case "--source-type":
					sourceType = GetValue(args, ref i, arg);
					break;

				case "--destination-type":
					destinationType = GetValue(args, ref i, arg);
					break;

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

		if (!showHelp)
		{
			if (string.IsNullOrWhiteSpace(source))
				throw new ArgumentException("Missing required argument '--source'.");

			if (string.IsNullOrWhiteSpace(destination))
				throw new ArgumentException("Missing required argument '--destination'.");

			if (string.IsNullOrWhiteSpace(sourceType))
				throw new ArgumentException("Missing required argument '--source-type'.");

			if (string.IsNullOrWhiteSpace(destinationType))
				throw new ArgumentException("Missing required argument '--destination-type'.");
		}

		return new MigrationOptions
		{
			Source = source,
			Destination = destination,
			SourceType = sourceType,
			DestinationType = destinationType,
			CleanDestination = cleanDestination,
			SkipConfirmation = skipConfirmation,
			ShowHelp = showHelp
		};
	}

	private static string GetValue(string[] args, ref int index, string arg)
	{
		if (index + 1 >= args.Length)
			throw new ArgumentException($"Missing value for argument '{arg}'.");

		index++;
		return args[index];
	}

	public static void PrintUsage()
	{
		Console.WriteLine("""
            CaveOfWonders.DbMigration

            Copies all data (pots, exchange rates, CPI, average wages, gems) from the
            source database into the destination database.

            Usage:
              dotnet CaveOfWonders.DbMigration.dll --source <connection-string> --source-type <type>
                  --destination <connection-string> --destination-type <type> [options]

            Options:
              --source <connection-string>        Connection string for the source database.
              --source-type <type>                Type of the source database (json, sqlite, litedb).
              --destination <connection-string>   Connection string for the destination database.
              --destination-type <type>           Type of the destination database (json, sqlite, litedb).
              --clean-destination, --clean        Delete all existing data in the destination
                                                   database before migrating.
              --yes, -y                           Do not prompt for confirmation when
                                                   --clean-destination is used.
              --help, -h                          Show this help message.
            """);
	}
}