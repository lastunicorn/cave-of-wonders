using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.PotEdit;

internal class PotEditView : ViewBase<PotEditViewModel>
{
	public override void Display(PotEditViewModel viewModel)
	{
		CustomConsole.WriteLineSuccess($"Pot '{viewModel.OldName}' was renamed to '{viewModel.NewName}'.");
	}
}
