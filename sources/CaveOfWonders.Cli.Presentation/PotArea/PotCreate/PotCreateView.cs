using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.PotCreate;

internal class PotCreateView : ViewBase<PotCreateViewModel>
{
	public override void Display(PotCreateViewModel viewModel)
	{
		CustomConsole.WriteLineSuccess($"Pot '{viewModel.PotName}' with currency '{viewModel.Currency}' created successfully.");

		DisplayPotDetails(viewModel);
	}

	private void DisplayPotDetails(PotCreateViewModel viewModel)
	{
		DataGrid dataGrid = DataGridTemplate.CreateNew();
		dataGrid.Title = "Pot Details";

		dataGrid.Columns.Add(new Column
		{
			ForegroundColor = ConsoleColor.White
		});

		dataGrid.Rows.Add("Id", viewModel.PotId);
		dataGrid.Rows.Add("Name", viewModel.PotName);

		if (!string.IsNullOrEmpty(viewModel.Description))
			dataGrid.Rows.Add("Description", viewModel.Description);

		dataGrid.Rows.Add("Start Date", viewModel.StartDate.ToString("d"));
		dataGrid.Rows.Add("Currency", viewModel.Currency);

		dataGrid.Display();
	}
}