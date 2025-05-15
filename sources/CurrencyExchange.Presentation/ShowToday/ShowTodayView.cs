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

using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;
using DustInTheWind.CurrencyExchange.Application.PresentToday;

namespace DustInTheWind.CurrencyExchange.Presentation.ShowToday;

internal class ShowTodayView : IView<PresentTodayResponse>
{
    public void Display(PresentTodayResponse response)
    {
        if (response.ExchangeRatesByDates != null)
        {
            DisplayByDates(response.ExchangeRatesByDates);
        }

        if (response.ExchangeRatesByCurrencies != null)
        {
            DisplayByCurrencies(response.ExchangeRatesByCurrencies);
        }

        if (response.ExchangeRatesByDates == null && response.ExchangeRatesByCurrencies == null)
        {
            CustomConsole.WriteLineWarning("There are no exchange rates for the date and/or currencies specified.");
        }
    }

    private void DisplayByDates(List<ExchangeRatesByDateCollection> exchangeRatesByDates)
    {
        DataGrid dataGrid = new("Exchange Rates")
        {
            TitleRow =
            {
                BackgroundColor = ConsoleColor.Gray,
                ForegroundColor = ConsoleColor.Black
            },
            BorderTemplate = BorderTemplate.SingleLineBorderTemplate
        };

        dataGrid.Columns.Add("Currency");

        foreach (ExchangeRatesByDateCollection exchangeRatesByDateCollection in exchangeRatesByDates)
        {
            dataGrid.Columns.Add(new Column(exchangeRatesByDateCollection.Date.ToString("d")));
            int columnIndex = dataGrid.Columns.Count - 1;

            foreach (ContentRow dataGridRow in dataGrid.Rows)
                dataGridRow.AddCell(string.Empty);

            foreach (ExchangeRateForCurrency exchangeRateForCurrency in exchangeRatesByDateCollection.ExchangeRates)
            {
                ContentRow contentRow = dataGrid.Rows.FirstOrDefault(x => x[0].Content == exchangeRateForCurrency.CurrencyPair);

                if (contentRow != null)
                {
                    contentRow[columnIndex].Content = new MultilineText(exchangeRateForCurrency.Value);
                }
                else
                {
                    contentRow = new ContentRow();
                    dataGrid.Rows.Add(contentRow);

                    contentRow.AddCell(exchangeRateForCurrency.CurrencyPair);

                    for (int i = 1; i < dataGrid.Columns.Count - 1; i++)
                        contentRow.AddCell(string.Empty);

                    contentRow.AddCell(exchangeRateForCurrency.Value);
                }
            }
        }

        dataGrid.Display();
    }

    private void DisplayByCurrencies(List<ExchangeRatesByCurrencyCollection> exchangeRatesByCurrencies)
    {
        DataGrid dataGrid = new("Exchange Rates")
        {
            TitleRow =
            {
                BackgroundColor = ConsoleColor.Gray,
                ForegroundColor = ConsoleColor.Black
            },
            BorderTemplate = BorderTemplate.SingleLineBorderTemplate
        };

        dataGrid.Columns.Add("Date");

        foreach (ExchangeRatesByCurrencyCollection exchangeRatesByCurrencyCollection in exchangeRatesByCurrencies)
        {
            dataGrid.Columns.Add(exchangeRatesByCurrencyCollection.CurrencyPair);
            int columnIndex = dataGrid.Columns.Count - 1;

            foreach (ContentRow dataGridRow in dataGrid.Rows)
                dataGridRow.AddCell(string.Empty);

            foreach (ExchangeRateForDate exchangeRateForDate in exchangeRatesByCurrencyCollection.ExchangeRates)
            {
                ContentRow contentRow = dataGrid.Rows.FirstOrDefault(x => x[0].Content == exchangeRateForDate.Date.ToString("d"));

                if (contentRow != null)
                {
                    contentRow[columnIndex].Content = new MultilineText(exchangeRateForDate.Value);
                }
                else
                {
                    contentRow = new ContentRow();
                    dataGrid.Rows.Add(contentRow);

                    contentRow.AddCell(exchangeRateForDate.Date.ToString("d"));

                    for (int i = 1; i < dataGrid.Columns.Count - 1; i++)
                        contentRow.AddCell(string.Empty);

                    contentRow.AddCell(exchangeRateForDate.Value);
                }
            }
        }

        dataGrid.Display();
    }
}