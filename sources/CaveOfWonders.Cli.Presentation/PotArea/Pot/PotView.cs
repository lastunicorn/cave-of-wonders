using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Pot;

internal class PotView : ViewBase<PotCommandViewModel>
{
    public override void Display(PotCommandViewModel viewModel)
    {
        bool isFirst = true;

        if (viewModel.PotDetailsViewModels == null || viewModel.PotDetailsViewModels.Count == 0)
        {
            CustomConsole.WriteLineWarning("There is no pot with the specified name or id.");
        }
        else
        {
            foreach (PotDetailsViewModel potDetailsViewModel in viewModel.PotDetailsViewModels)
            {
                if (isFirst)
                    isFirst = false;
                else
                    Console.WriteLine();

                DisplayPotDetails(potDetailsViewModel);
            }
        }
    }

    private static void DisplayPotDetails(PotDetailsViewModel potDetailsViewModel)
    {
        DataGrid dataGrid = DataGridTemplate.CreateNew();
        dataGrid.Title = potDetailsViewModel.Name;

        if (!potDetailsViewModel.IsActive)
        {
            DataGridTemplate.Disable(dataGrid);
        }
        else
        {
            dataGrid.Columns.Add(new Column
            {
                ForegroundColor = ConsoleColor.White
            });
        }

        dataGrid.Rows.Add("Id", potDetailsViewModel.Id);
        dataGrid.Rows.Add("Description", potDetailsViewModel.Description);
        dataGrid.Rows.Add("StartDate", potDetailsViewModel.StartDate.ToString("d"));
        dataGrid.Rows.Add("EndDate", potDetailsViewModel.EndDate?.ToString("d") ?? string.Empty);
        dataGrid.Rows.Add("Currency", potDetailsViewModel.Currency);
        dataGrid.Rows.Add("Labels", string.Join(", ", potDetailsViewModel.Labels));
        dataGrid.Rows.Add("Snapshot Count", potDetailsViewModel.SnapshotCount);
        dataGrid.Rows.Add("Last Snapshot", $"{potDetailsViewModel.LastSnapshotDate:d} ({potDetailsViewModel.Value.ToDisplayString()})");

        dataGrid.Display();
    }
}