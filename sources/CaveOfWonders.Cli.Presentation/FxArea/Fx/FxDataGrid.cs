using System.Globalization;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentExchangeRate;
using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.FxArea.Fx;

internal class FxDataGrid : DataGrid
{
	private readonly List<CurrencyPair> currencyPairs = [];

	public FxDataGrid(FxViewModel viewModel)
	{
		Title = "Exchange Rates";
		TitleRow.BackgroundColor = ConsoleColor.Gray;
		TitleRow.ForegroundColor = ConsoleColor.Black;
		BorderTemplate = BorderTemplate.SingleLineBorderTemplate;

		Columns.Add("Date");

		AddRows(viewModel);
	}

	private void AddRows(FxViewModel viewModel)
	{
		IEnumerable<ContentRow> rows = viewModel.DailyExchangeRates
			.Select(CreateRow);

		Rows.AddRange(rows);
	}

	private ContentRow CreateRow(DailyExchangeRates dailyExchangeRates)
	{
		ContentRow row = new();

		string date = dailyExchangeRates.Date.ToString("d", CultureInfo.CurrentCulture);
		row.AddCell(date);

		Dictionary<CurrencyPair, ContentCell> cellsByCurrencyPairs = dailyExchangeRates.ExchangeRates
			.ToDictionary(
				x => x.CurrencyPair,
				x => new ContentCell(x.Value.ToString(CultureInfo.CurrentCulture)));

		foreach (CurrencyPair currencyPair in currencyPairs)
		{
			bool exists = cellsByCurrencyPairs.TryGetValue(currencyPair, out ContentCell cell);

			if (exists)
			{
				row.AddCell(cell);
				cellsByCurrencyPairs.Remove(currencyPair);
			}
			else
			{
				row.AddCell(string.Empty);
			}
		}

		foreach (KeyValuePair<CurrencyPair, ContentCell> pair in cellsByCurrencyPairs)
		{
			AddCurrencyColumn(pair.Key);
			row.AddCell(pair.Value);
		}

		return row;
	}

	private void AddCurrencyColumn(CurrencyPair currencyPair)
	{
		Columns.Add(currencyPair);
		currencyPairs.Add(currencyPair);
	}
}