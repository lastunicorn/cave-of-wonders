using DustInTheWind.CaveOfWonders.Cli.Application.PresentGems;
using DustInTheWind.CaveOfWonders.DataTypes;
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

        decimal totalAmount = 0;

        foreach (GemDto gemDto in viewModel.Gems)
        {
            dataGrid.Rows.Add(gemDto.Date, gemDto.Category, gemDto.Amount);

            if (gemDto.Category is GemCategory.Gain or GemCategory.Loss)
                totalAmount += gemDto.Amount;
        }

        dataGrid.Footer = $"Total amount: {totalAmount}";

        dataGrid.Display();
    }
}