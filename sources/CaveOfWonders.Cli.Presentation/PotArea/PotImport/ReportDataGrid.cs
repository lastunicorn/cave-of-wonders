using DustInTheWind.CaveOfWonders.Cli.Application.ImportPotSnapshots.Importing;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.PotImport;

internal class ReportDataGrid : DataGrid
{
	public ReportDataGrid()
	{
		_ = Columns.Add(new Column("Id")
		{
			ForegroundColor = ConsoleColor.DarkGray
		});

		_ = Columns.Add("Pot");

		_ = Columns.Add(new Column("Added")
		{
			CellHorizontalAlignment = DustInTheWind.ConsoleTools.Controls.HorizontalAlignment.Right,
			ForegroundColor = ConsoleColor.Green
		});

		_ = Columns.Add(new Column("Skipped\n(exists)")
		{
			CellHorizontalAlignment = DustInTheWind.ConsoleTools.Controls.HorizontalAlignment.Right
		});

		_ = Columns.Add(new Column("Skipped\n(pot not active)")
		{
			CellHorizontalAlignment = DustInTheWind.ConsoleTools.Controls.HorizontalAlignment.Right,
			ForegroundColor = ConsoleColor.DarkYellow
		});
	}

	public void AddRows(IEnumerable<PotImportReport> report)
	{
		IEnumerable<ContentRow> rows = report.Select(CreateRow);
		Rows.AddRange(rows);
	}

	private static ContentRow CreateRow(PotImportReport potImportReport)
	{
		string id = potImportReport.PotId.ToString("N")[..8];

		string potName = potImportReport.PotName;

		string addedCount = potImportReport.AddCount == 0
			? string.Empty
			: potImportReport.AddCount.ToString();

		string skippedCount = potImportReport.SkipExistsCount == 0
			? string.Empty
			: potImportReport.SkipExistsCount.ToString();

		string skippedNotActiveCount = potImportReport.SkipNotActiveCount == 0
			? string.Empty
			: potImportReport.SkipNotActiveCount.ToString();

		ContentRow contentRow = new(id, potName, addedCount, skippedCount, skippedNotActiveCount);

		if (potName == null)
			contentRow.ForegroundColor = ConsoleColor.DarkRed;

		return contentRow;
	}
}