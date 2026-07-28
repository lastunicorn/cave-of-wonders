using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.AppArea.About;

internal class AboutView : IView<AboutViewModel>
{
    public void Display(AboutViewModel viewModel)
    {
        CustomConsole.WriteLine(viewModel.ApplicationName);
        CustomConsole.WriteLine(viewModel.Description);
        Console.WriteLine();
        CustomConsole.WriteLine($"Version: {viewModel.Version}");
        CustomConsole.WriteLine($"Author: {viewModel.Author}");
        CustomConsole.WriteLine($"Database: {viewModel.DatabaseLocation}");
    }
}
