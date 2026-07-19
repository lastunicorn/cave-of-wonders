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
			Footer = $"Gain: {viewModel.TotalGain} EUR"
		};

		dataGrid.Columns.Add("Pot");
		dataGrid.Columns.Add("Currency");
		dataGrid.Columns.Add(new Column("Gain")
		{
			CellHorizontalAlignment = HorizontalAlignment.Right
		});
		dataGrid.Columns.Add(new Column("Normalized Gain")
		{
			CellHorizontalAlignment = HorizontalAlignment.Right
		});

		foreach (GainItem item in viewModel.Items)
			dataGrid.Rows.Add(item.PotName, item.Currency, item.Gain, item.NormalizedGain);

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