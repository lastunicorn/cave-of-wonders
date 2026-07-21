using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation;

internal class DataGridBuilder
{
	private readonly DataGrid dataGrid;

	public static DataGridBuilder Create()
	{
		return new DataGridBuilder();
	}

	public DataGridBuilder()
	{
		dataGrid = new DataGrid
		{
			TitleRow =
			{
				BackgroundColor = ConsoleColor.Gray,
				ForegroundColor = ConsoleColor.Black
			}
		};
	}

	public DataGridBuilder Title(string title)
	{
		dataGrid.Title = title;

		return this;
	}

	public DataGridBuilder Enabled(bool value)
	{
		if (!value)
		{
			dataGrid.TitleRow.BackgroundColor = ConsoleColor.DarkGray;
			dataGrid.Border.ForegroundColor = ConsoleColor.DarkGray;
			dataGrid.ForegroundColor = ConsoleColor.DarkGray;
		}

		return this;
	}

	public DataGridBuilder Columns(Func<IEnumerable<Column>> action)
	{
		if (action == null) throw new ArgumentNullException(nameof(action));

		IEnumerable<Column> columns = action();

		foreach (Column column in columns)
			dataGrid.Columns.Add(column);

		return this;
	}

	public DataGridBuilder Column(string name)
	{
		dataGrid.Columns.Add(name);

		return this;
	}

	public DataGridBuilder Column(Column column)
	{
		dataGrid.Columns.Add(column);

		return this;
	}

	public DataGridBuilder Rows(Func<IEnumerable<ContentRow>> action)
	{
		if (action == null) throw new ArgumentNullException(nameof(action));

		IEnumerable<ContentRow> rows = action();

		foreach (ContentRow row in rows)
			dataGrid.Rows.Add(row);

		return this;
	}

	public DataGridBuilder Row(ContentRow row)
	{
		dataGrid.Rows.Add(row);

		return this;
	}

	public DataGridBuilder Footer(string text)
	{
		dataGrid.FooterRow.FooterCell.Content = text;

		return this;
	}

	public DataGrid Build()
	{
		return dataGrid;
	}
}