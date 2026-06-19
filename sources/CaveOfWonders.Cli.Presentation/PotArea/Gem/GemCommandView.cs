using DustInTheWind.CaveOfWonders.Cli.Application.PresentGems;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;

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
            dataGrid.Rows.Add(gemDto.Date, gemDto.Category, gemDto.Amount);

        decimal totalAmount = viewModel.CalculateTotal();
        dataGrid.Footer = $"Total amount: {totalAmount}";

        dataGrid.Display();
    }
}