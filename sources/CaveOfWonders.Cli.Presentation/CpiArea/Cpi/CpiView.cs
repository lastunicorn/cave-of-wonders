using DustInTheWind.CaveOfWonders.Cli.Application.PresentCpi;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.CpiArea.Cpi;

internal class CpiView : IView<CpiViewModel>
{
	public void Display(CpiViewModel viewModel)
	{
		if (viewModel.Records?.Count > 0)
		{
			DataGrid dataGrid = new();

			dataGrid.Columns.Add("Year");
			dataGrid.Columns.Add("Value %", HorizontalAlignment.Right);

			foreach (CpiDto cpiDto in viewModel.Records)
			{
				dataGrid.Rows.Add(cpiDto.Year, cpiDto.Value);
			}

			dataGrid.Display();
		}
		else
		{
			CustomConsole.WriteLineWarning("No inflation rates.");
		}
	}
}