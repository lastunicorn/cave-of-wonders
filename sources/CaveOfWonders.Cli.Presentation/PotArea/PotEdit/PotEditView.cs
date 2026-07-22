using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.PotEdit;

internal class PotEditView : ViewBase<PotEditViewModel>
{
	public override void Display(PotEditViewModel viewModel)
	{
		bool anyUpdated = viewModel.NameUpdated || viewModel.DescriptionUpdated || viewModel.CurrencyUpdated ||
		                  viewModel.StartDateUpdated || viewModel.EndDateUpdated;

		if (!anyUpdated)
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

		if (viewModel.StartDateUpdated)
			CustomConsole.WriteLineSuccess($"Pot '{viewModel.PotName}' start date was changed from '{viewModel.OldStartDate:d}' to '{viewModel.NewStartDate:d}'.");

		if (viewModel.EndDateUpdated)
			CustomConsole.WriteLineSuccess($"Pot '{viewModel.PotName}' end date was changed from '{FormatDate(viewModel.OldEndDate)}' to '{FormatDate(viewModel.NewEndDate)}'.");
	}

	private static string FormatDate(DateOnly? date)
	{
		return date?.ToString("d") ?? "none";
	}
}
