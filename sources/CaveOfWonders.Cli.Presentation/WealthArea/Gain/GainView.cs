using DustInTheWind.CaveOfWonders.Cli.Application.CalculateGain;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.WealthArea.Gain;

internal class GainView : IView<GainViewModel>
{
    public void Display(GainViewModel viewModel)
    {
        DataGrid dataGrid = new();

        dataGrid.Columns.Add("Pot");
        dataGrid.Columns.Add("Currency");
        dataGrid.Columns.Add(new Column("Total Gain")
        {
            CellHorizontalAlignment = HorizontalAlignment.Right
        });

        foreach (GainItem item in viewModel.Items)
            dataGrid.Rows.Add(item.PotName, item.Currency, item.TotalGain);

        dataGrid.Display();
    }
}