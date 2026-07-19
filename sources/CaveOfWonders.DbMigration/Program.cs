using System.CommandLine;

namespace DustInTheWind.CaveOfWonders.DbMigration;

internal static class Program
{
	private static async Task<int> Main(string[] args)
	{
		RootCommand rootCommand = MigrateCommand.Create();
		ParseResult parseResult = rootCommand.Parse(args);

		return await parseResult.InvokeAsync();
	}
}