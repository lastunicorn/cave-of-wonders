using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation;

internal static class DataGridExtensions
{
	public static DataGrid Disable(this DataGrid dataGrid)
	{
		if (dataGrid == null)
			return null;

		dataGrid.TitleRow.BackgroundColor = ConsoleColor.DarkGray;
		dataGrid.Border.ForegroundColor = ConsoleColor.DarkGray;
		dataGrid.ForegroundColor = ConsoleColor.DarkGray;

		return dataGrid;
	}
}