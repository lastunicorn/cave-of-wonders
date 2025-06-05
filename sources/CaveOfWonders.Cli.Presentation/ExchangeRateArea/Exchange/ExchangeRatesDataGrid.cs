// Cave of Wonders
// Copyright (C) 2023-2024 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using DustInTheWind.CaveOfWonders.Cli.Application.PresentExchangeRate;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.ConsoleTools.Controls.Tables;
using System.Globalization;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.ExchangeRateArea.Exchange;

internal class ExchangeRatesDataGrid : DataGrid
{
    private readonly List<CurrencyPair> currencyPairs = [];

    public ExchangeRatesDataGrid(PresentExchangeRateResponse response)
    {
        Title = "Exchange Rates";
        TitleRow.BackgroundColor = ConsoleColor.Gray;
        TitleRow.ForegroundColor = ConsoleColor.Black;
        BorderTemplate = BorderTemplate.SingleLineBorderTemplate;

        Columns.Add("Date");

        AddRows(response);
    }

    private void AddRows(PresentExchangeRateResponse response)
    {
        foreach (DailyExchangeRates dailyExchangeRates in response.DailyExchangeRates)
        {
            ContentRow row = CreateRow(dailyExchangeRates);
            Rows.Add(row);
        }
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