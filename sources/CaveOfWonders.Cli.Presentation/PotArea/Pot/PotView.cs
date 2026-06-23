using DustInTheWind.CaveOfWonders.Cli.Presentation.Controls;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Pot;

internal class PotView : ViewBase<PotCommandViewModel>
{
    public override void Display(PotCommandViewModel viewModel)
    {
        bool potExists = viewModel.PotDetails?.Count > 0 || viewModel.PotSummaries?.Count > 0;

        if (!potExists)
        {
            CustomConsole.WriteLineWarning("There is no pot with the specified name or id.");
        }
        else if (viewModel.PotDetails?.Count > 0)
        {
            bool isFirst = true;

            foreach (PotDetailsViewModel potDetailsViewModel in viewModel.PotDetails)
            {
                if (isFirst)
                    isFirst = false;
                else
                    Console.WriteLine();

                DisplayPotDetails(potDetailsViewModel);
            }
        }
        else if (viewModel.PotSummaries?.Count > 0)
        {
            DisplayPotSummary(viewModel);
        }
    }

    private static void DisplayPotDetails(PotDetailsViewModel potDetailsViewModel)
    {
        DataGrid dataGrid = DataGridTemplate.CreateNew();
        dataGrid.Title = potDetailsViewModel.Name;

        if (!potDetailsViewModel.IsActive)
        {
            dataGrid.Disable();
        }
        else
        {
            dataGrid.Columns.Add(new Column
            {
                ForegroundColor = ConsoleColor.White
            });
            
            dataGrid.Columns.Add(new Column
            {
                ForegroundColor = ConsoleColor.Gray
            });
        }

        dataGrid.Rows.Add("Id", potDetailsViewModel.Id);
        dataGrid.Rows.Add("Description", potDetailsViewModel.Description);
        dataGrid.Rows.Add("StartDate", potDetailsViewModel.StartDate.ToString("d"));
        dataGrid.Rows.Add("EndDate", potDetailsViewModel.EndDate?.ToString("d") ?? string.Empty);
        dataGrid.Rows.Add("Currency", potDetailsViewModel.Currency);
        dataGrid.Rows.Add("Labels", string.Join(", ", potDetailsViewModel.Labels));
        dataGrid.Rows.Add("Snapshot Count", potDetailsViewModel.SnapshotCount);

        string lastSnapshot = potDetailsViewModel.LastSnapshotDate != null
            ? $"{potDetailsViewModel.LastSnapshotDate:d} ({potDetailsViewModel.Value.ToDisplayString()})"
            : string.Empty;

        dataGrid.Rows.Add("Last Snapshot", lastSnapshot);

        dataGrid.Display();
    }

    private static void DisplayPotSummary(PotCommandViewModel viewModel)
    {
        DataGrid dataGrid = DataGridTemplate.CreateNew();
        dataGrid.Title = "Pots";

        dataGrid.Columns.Add(new Column("Id")
        {
            ForegroundColor = ConsoleColor.DarkGray,
            CellContentOverflow = CellContentOverflow.PreserveOverflow
        });

        dataGrid.Columns.Add("Name");
        dataGrid.Columns.Add("Currency");

        foreach (PotSummaryViewModel potSummaryViewModel in viewModel.PotSummaries)
        {
            ShortPotId id = potSummaryViewModel.Id;
            string name = potSummaryViewModel.Name;
            string currency = potSummaryViewModel.Currency;

            ContentRow row = new(id, name, currency);

            if (!potSummaryViewModel.IsActive)
            {
                row[1].ForegroundColor = ConsoleColor.DarkGray;
                row[2].ForegroundColor = ConsoleColor.DarkGray;
            }

            dataGrid.Rows.Add(row);
        }

        dataGrid.Display();
    }
}