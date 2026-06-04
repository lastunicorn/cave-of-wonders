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

using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;
using DustInTheWind.CurrencyExchange.Application.PresentToday;

namespace DustInTheWind.CurrencyExchange.Presentation.ShowToday;

internal class ExchangeRatesByCurrencyControl
{
    private DataGrid dataGrid;

    public List<ExchangeRatesByCurrencyCollection> Items { get; set; }

    public void Display()
    {
        CreateNewDataGrid();

        foreach (ExchangeRatesByCurrencyCollection exchangeRatesByCurrencyCollection in Items)
        {
            CreateColumn(exchangeRatesByCurrencyCollection.CurrencyPair);

            foreach (ExchangeRateForDate exchangeRateForDate in exchangeRatesByCurrencyCollection.ExchangeRates)
            {
                ContentRow contentRow = GetOrCreateRow(exchangeRateForDate.Date);
                contentRow.Last().Content = new MultilineText(exchangeRateForDate.Value);
            }
        }

        dataGrid.Display();
    }

    private void CreateNewDataGrid()
    {
        dataGrid = new DataGrid("Exchange Rates")
        {
            TitleRow =
            {
                BackgroundColor = ConsoleColor.Gray,
                ForegroundColor = ConsoleColor.Black
            },
            BorderTemplate = BorderTemplate.SingleLineBorderTemplate
        };

        dataGrid.Columns.Add("Date");
    }

    private void CreateColumn(CurrencyPair currencyPair)
    {
        dataGrid.Columns.Add(currencyPair);

        foreach (ContentRow dataGridRow in dataGrid.Rows)
            dataGridRow.AddCell(string.Empty);
    }

    internal ContentRow GetOrCreateRow(DateTime date)
    {
        ContentRow contentRow = dataGrid.Rows.FirstOrDefault(x => x[0].Content == date.ToString("d"));

        if (contentRow == null)
        {
            contentRow = new ContentRow();
            dataGrid.Rows.Add(contentRow);

            contentRow.AddCell(date.ToString("d"));

            for (int i = 1; i <= dataGrid.Columns.Count - 1; i++)
                contentRow.AddCell(string.Empty);
        }

        return contentRow;
    }
}