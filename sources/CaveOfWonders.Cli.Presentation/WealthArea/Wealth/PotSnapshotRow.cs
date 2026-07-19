using DustInTheWind.CaveOfWonders.Cli.Presentation.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.WealthArea.Wealth;

internal class PotSnapshotRow : ContentRow
{
	private PotSnapshotViewModel viewModel;

	public PotSnapshotViewModel ViewModel
	{
		get => viewModel;
		set
		{
			viewModel = value;
			RegenerateCells();
		}
	}

	private void RegenerateCells()
	{
		ShortPotId id = viewModel.Id;
		AddCell(id);

		string name = viewModel.Name;
		AddCell(name);

		string value = viewModel.OriginalValue?.ToDisplayString();
		ContentCell valueCell = AddCell(value);

		string date = viewModel.Date?.ToString("d");
		ContentCell dateCell = AddCell(date);

		string normalizedValue = viewModel.NormalizedValue?.ToDisplayString();
		ContentCell normalizedValueCell = AddCell(normalizedValue);

		if (viewModel.IsPotActive)
		{
			if (!viewModel.IsValueActual)
			{
				valueCell.ForegroundColor = ConsoleColor.DarkYellow;
				dateCell.ForegroundColor = ConsoleColor.DarkYellow;
			}

			if (!viewModel.IsValueAlreadyNormal)
				valueCell.ForegroundColor = ConsoleColor.DarkGray;
		}
		else
		{
			ForegroundColor = ConsoleColor.DarkGray;
		}

		bool isNormalizedValue = viewModel.NormalizedValue?.Value != 0 &&
			!viewModel.IsValueAlreadyNormal &&
			!viewModel.IsNormalizedCurrent;

		if (isNormalizedValue)
			normalizedValueCell.ForegroundColor = ConsoleColor.DarkYellow;
	}
}