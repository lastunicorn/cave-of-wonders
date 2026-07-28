using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.AppArea.About;

internal class AboutView : IView<AboutViewModel>
{
	public void Display(AboutViewModel viewModel)
	{
		CustomConsole.WriteLine(ConsoleColor.White, viewModel.ApplicationName);
		CustomConsole.WriteLine(ConsoleColor.Gray, viewModel.Description);
		Console.WriteLine();
		CustomConsole.Write("Version: ");
		CustomConsole.WriteLine(ConsoleColor.Gray, viewModel.Version);

		CustomConsole.Write("Author: ");
		CustomConsole.WriteLine(ConsoleColor.Gray, viewModel.Author);

		CustomConsole.Write("Database: ");
		CustomConsole.WriteLine(ConsoleColor.Gray, viewModel.DatabaseLocation);
	}
}