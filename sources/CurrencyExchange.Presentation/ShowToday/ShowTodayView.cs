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
using DustInTheWind.CurrencyExchange.Application.PresentToday;

namespace DustInTheWind.CurrencyExchange.Presentation.ShowToday;

internal class ShowTodayView : IView<PresentTodayResponse>
{
    public void Display(PresentTodayResponse response)
    {
        if (response.ExchangeRatesByDates != null)
            Display(response.ExchangeRatesByDates);

        if (response.ExchangeRatesByCurrencies != null)
            Display(response.ExchangeRatesByCurrencies);

        if (response.ExchangeRatesByDates == null && response.ExchangeRatesByCurrencies == null)
            CustomConsole.WriteLineWarning("There are no exchange rates for the date and/or currencies specified.");
    }

    private static void Display(List<ExchangeRatesByDateCollection> exchangeRatesByDate)
    {
        ExchangeRatesByDateControl exchangeRatesByDateControl = new()
        {
            Items = exchangeRatesByDate
        };

        exchangeRatesByDateControl.Display();
    }

    private static void Display(List<ExchangeRatesByCurrencyCollection> exchangeRatesByCurrency)
    {
        ExchangeRatesByCurrencyControl exchangeRatesByCurrencyControl = new()
        {
            Items = exchangeRatesByCurrency
        };

        exchangeRatesByCurrencyControl.Display();
    }
}