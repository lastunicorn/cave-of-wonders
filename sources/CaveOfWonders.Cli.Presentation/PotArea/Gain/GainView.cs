using DustInTheWind.CaveOfWonders.Cli.Application;
using DustInTheWind.CaveOfWonders.Cli.Application.CalculateGain;
using DustInTheWind.CaveOfWonders.Cli.Presentation.Controls;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Gain;

internal class GainView : IView<GainViewModel>
{
	public void Display(GainViewModel viewModel)
	{
		DisplayGainItems(viewModel);
		DisplayConversionRates(viewModel);
	}

	private static void DisplayGainItems(GainViewModel viewModel)
	{
		DataGrid dataGrid = new()
		{
			Footer = $"Gain: {viewModel.TotalGain.ToDisplayString()}"
		};

		dataGrid.Columns.Add("Pot");
		dataGrid.Columns.Add(new Column("Gain")
		{
			CellHorizontalAlignment = HorizontalAlignment.Right
		});
		dataGrid.Columns.Add(new Column("Normalized Gain")
		{
			CellHorizontalAlignment = HorizontalAlignment.Right
		});

		foreach (GainItem item in viewModel.Items)
		{
			string potName = item.PotName;
			string gainAsString = item.Gain.ToDisplayString();
			string normalizedGainAsString = item.NormalizedGain.ToDisplayString();

			ContentRow row = dataGrid.Rows.Add(potName, gainAsString, normalizedGainAsString);

			if (!item.IsActual)
				row[1].ForegroundColor = ConsoleColor.DarkYellow;
		}

		dataGrid.Display();
	}

	private static void DisplayConversionRates(GainViewModel viewModel)
	{
		if (viewModel.ConversionRates.Count == 0)
			return;

		Console.WriteLine();

		ExchangeRatesControl exchangeRatesControl = new()
		{
			ExchangeRates = viewModel.ConversionRates
		};

		exchangeRatesControl.Display();
	}
}