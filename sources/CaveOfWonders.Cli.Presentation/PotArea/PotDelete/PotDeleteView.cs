using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.PotDelete;

internal class PotDeleteView : ViewBase<PotDeleteViewModel>
{
    public override void Display(PotDeleteViewModel viewModel)
    {
        if (viewModel.PotFound)
            CustomConsole.WriteLineSuccess($"Pot '{viewModel.PotName}' was deleted successfully.");
        else
            CustomConsole.WriteLineWarning("There is no pot with the specified name or id.");
    }
}
