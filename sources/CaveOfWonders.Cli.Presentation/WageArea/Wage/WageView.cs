using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.WageArea.Wage;

internal class WageView : ViewBase<WagesViewModel>
{
	public override void Display(WagesViewModel viewModel)
	{
		DataGrid dataGrid = new()
		{
			Title = "Average Wage"
		};

		dataGrid.Columns.Add("Year");
		dataGrid.Columns.Add("Gross", HorizontalAlignment.Right);
		dataGrid.Columns.Add("Net", HorizontalAlignment.Right);

		foreach (WageViewModel viewModelWage in viewModel.Wages.OrderBy(x => x.Year))
		{
			int year = viewModelWage.Year;
			string gross = viewModelWage.GrossValue.HasValue
				? viewModelWage.GrossValue.Value.ToString("N2")
				: string.Empty;
			string net = viewModelWage.NetValue.HasValue
				? viewModelWage.NetValue.Value.ToString("N2")
				: string.Empty;

			dataGrid.Rows.Add(year, gross, net);
		}

		dataGrid.Display();
	}
}