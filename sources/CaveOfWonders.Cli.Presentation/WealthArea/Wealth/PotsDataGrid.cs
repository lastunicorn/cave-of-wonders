using DustInTheWind.CaveOfWonders.Cli.Application;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.WealthArea.Wealth;

internal class PotsDataGrid
{
	public DateOnly Date { get; set; }

	public List<PotSnapshotViewModel> Values { get; set; }

	public CurrencyValue Total { get; set; }

	public void Display()
	{
		DataGrid dataGrid = DataGridTemplate.CreateNew();
		dataGrid.Title = $"Pots - {Date:d} ({Total.Currency})";

		AddColumns(dataGrid);
		AddRows(dataGrid);
		AddFooter(dataGrid);

		dataGrid.Display();
	}

	private static void AddColumns(DataGrid dataGrid)
	{
		dataGrid.Columns.Add(new Column("Id")
		{
			ForegroundColor = ConsoleColor.DarkGray,
			CellContentOverflow = CellContentOverflow.PreserveOverflow
		});

		dataGrid.Columns.Add(new Column("Name"));

		dataGrid.Columns.Add(new Column("Value")
		{
			CellHorizontalAlignment = HorizontalAlignment.Right,
			CellContentOverflow = CellContentOverflow.PreserveOverflow
		});

		dataGrid.Columns.Add(new Column("Date")
		{
			CellHorizontalAlignment = HorizontalAlignment.Right,
			CellContentOverflow = CellContentOverflow.PreserveOverflow
		});

		dataGrid.Columns.Add(new Column("Normalized Value")
		{
			CellHorizontalAlignment = HorizontalAlignment.Right,
			CellContentOverflow = CellContentOverflow.PreserveOverflow,
			HeaderCell =
			{
				ContentOverflow = CellContentOverflow.WrapWord
			}
		});
	}

	private void AddRows(DataGrid dataGrid)
	{
		IEnumerable<ContentRow> rows = Values
			.Select(static x => new PotSnapshotRow
			{
				ViewModel = x
			});

		dataGrid.Rows.AddRange(rows);
	}

	private void AddFooter(DataGrid dataGrid)
	{
		string totalText = $"Total: {Total.ToDisplayString()}";

		dataGrid.Footer = totalText;
		dataGrid.FooterRow.ForegroundColor = ConsoleColor.White;
	}
}