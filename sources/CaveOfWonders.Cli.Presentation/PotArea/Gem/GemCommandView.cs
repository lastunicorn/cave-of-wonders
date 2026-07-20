using DustInTheWind.CaveOfWonders.Cli.Application;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentGems;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;
using System.Globalization;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Gem;

internal class GemCommandView : IView<GemCommandViewModel>
{
	public void Display(GemCommandViewModel viewModel)
	{
		DataGrid dataGrid = new();

		dataGrid.Columns.Add("Date");
		dataGrid.Columns.Add("Category");
		dataGrid.Columns.Add("Amount", HorizontalAlignment.Right);

		foreach (GemDto gemDto in viewModel.Gems)
		{
			string date = gemDto.Date.ToString(CultureInfo.CurrentUICulture);
			string category = gemDto.Category.ToString();
			string amount = gemDto.Amount.ToDisplayString();

			ContentRow contentRow = dataGrid.Rows.Add(date, category, amount);

			if (gemDto.Amount.Value < 0)
				contentRow[2].ForegroundColor = ConsoleColor.DarkRed;
		}

		Amount totalAmount = viewModel.TotalAmount;
		dataGrid.Footer = $"Total amount: {totalAmount.ToDisplayString()}";

		dataGrid.Display();
	}
}