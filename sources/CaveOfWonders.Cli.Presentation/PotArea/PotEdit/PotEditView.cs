using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.PotEdit;

internal class PotEditView : ViewBase<PotEditViewModel>
{
	public override void Display(PotEditViewModel viewModel)
	{
		if (!viewModel.NameUpdated && !viewModel.DescriptionUpdated && !viewModel.CurrencyUpdated)
		{
			CustomConsole.WriteLineWarning("No property was updated.");
			return;
		}

		if (viewModel.NameUpdated)
			CustomConsole.WriteLineSuccess($"Pot '{viewModel.OldName}' was renamed to '{viewModel.NewName}'.");

		if (viewModel.DescriptionUpdated)
			CustomConsole.WriteLineSuccess($"Pot '{viewModel.PotName}' description was updated.");

		if (viewModel.CurrencyUpdated)
			CustomConsole.WriteLineSuccess($"Pot '{viewModel.PotName}' currency was changed from '{viewModel.OldCurrency}' to '{viewModel.NewCurrency}'.");
	}
}
