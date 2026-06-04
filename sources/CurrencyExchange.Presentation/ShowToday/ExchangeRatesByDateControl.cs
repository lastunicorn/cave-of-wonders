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

internal class ExchangeRatesByDateControl
{
    private DataGrid dataGrid;

    public List<ExchangeRatesByDateCollection> Items { get; set; }

    public void Display()
    {
        CreateNewDataGrid();

        if (Items != null)
        {
            foreach (ExchangeRatesByDateCollection exchangeRatesByDateCollection in Items)
            {
                CreateColumn(exchangeRatesByDateCollection.Date);

                foreach (ExchangeRateForCurrency exchangeRateForCurrency in exchangeRatesByDateCollection.ExchangeRates)
                {
                    ContentRow contentRow = GetOrCreateRow(exchangeRateForCurrency.CurrencyPair);
                    contentRow.Last().Content = new MultilineText(exchangeRateForCurrency.Value);
                }
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

        dataGrid.Columns.Add("Currency");
    }

    private void CreateColumn(DateTime date)
    {
        Column newColumn = new(date.ToString("d"));
        Column column = dataGrid.Columns.Add(newColumn);

        if (date.DayOfWeek is DayOfWeek.Sunday or DayOfWeek.Saturday)
            column.ForegroundColor = ConsoleColor.DarkGray;

        foreach (ContentRow dataGridRow in dataGrid.Rows)
            dataGridRow.AddCell(string.Empty);
    }

    private ContentRow GetOrCreateRow(CurrencyPair currencyPair)
    {
        ContentRow contentRow = dataGrid.Rows.FirstOrDefault(x => x[0].Content == currencyPair);

        if (contentRow == null)
        {
            contentRow = new ContentRow();
            dataGrid.Rows.Add(contentRow);

            contentRow.AddCell(currencyPair);

            for (int i = 1; i <= dataGrid.Columns.Count - 1; i++)
                contentRow.AddCell(string.Empty);
        }

        return contentRow;
    }
}