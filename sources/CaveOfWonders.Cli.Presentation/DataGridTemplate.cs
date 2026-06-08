using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation;

internal static class DataGridTemplate
{
    public static DataGrid CreateNew()
    {
        return new DataGrid
        {
            TitleRow =
            {
                BackgroundColor = ConsoleColor.Gray,
                ForegroundColor = ConsoleColor.Black
            }
        };
    }

    public static DataGrid Disable(DataGrid dataGrid)
    {
        dataGrid.TitleRow.BackgroundColor = ConsoleColor.DarkGray;
        dataGrid.Border.ForegroundColor = ConsoleColor.DarkGray;
        dataGrid.ForegroundColor = ConsoleColor.DarkGray;

        return dataGrid;
    }
}