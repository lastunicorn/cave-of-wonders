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

using System.Globalization;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentExchangeRate;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.Exchange;

internal class ExchangeView : IView<PresentExchangeRateResponse>
{
    public void Display(PresentExchangeRateResponse response)
    {
        if (response.ExchangeRates.Count == 0)
        {
            CustomConsole.WriteLineWarning($"There are no exchange rates for {response.CurrencyPair} and the specified dates.");
        }
        else
        {
            DataGrid dataGrid = new("Exchange Rates")
            {
                TitleRow =
                {
                    BackgroundColor = ConsoleColor.Gray,
                    ForegroundColor = ConsoleColor.Black
                },
                Border =
                {
                    Template = BorderTemplate.SingleLineBorderTemplate
                }
            };

            dataGrid.Columns.Add("Date");
            dataGrid.Columns.Add(response.CurrencyPair);

            foreach (ExchangeRateResponseDto exchangeRateResponseDto in response.ExchangeRates)
            {
                string date = exchangeRateResponseDto.Date.ToString("d", CultureInfo.CurrentCulture);
                string value = exchangeRateResponseDto.Value.ToString(CultureInfo.CurrentCulture);
                dataGrid.Rows.Add(date, value);
            }

            dataGrid.Display();
        }

        if (response.Comments != null)
            CustomConsole.WriteLineWarning(response.Comments);
    }
}