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
}