using DustInTheWind.CaveOfWonders.Cli.Application.PresentInflation;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.CpiArea.Inflation;

internal class InflationView : IView<InflationViewModel>
{
	public void Display(InflationViewModel viewModel)
	{
		if (viewModel.Records?.Count > 0)
		{
			foreach (InflationDto cpiDto in viewModel.Records)
			{
				Console.Write($"{cpiDto.Year}: {cpiDto.Value,6:N2} ");
				DisplayChartLine(cpiDto.Value);

				Console.WriteLine();
			}
		}
		else
		{
			CustomConsole.WriteLineWarning("No inflation rates.");
		}
	}

	private static void DisplayChartLine(decimal roundedValue)
	{
		ChartLineControl chartLineControl = new()
		{
			Value = roundedValue
		};
		chartLineControl.Display();
	}
}