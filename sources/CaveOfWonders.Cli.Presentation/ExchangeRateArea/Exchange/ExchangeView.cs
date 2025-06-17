// Cave of Wonders
// Copyright (C) 2023-2025 Dust in the Wind
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
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.ExchangeRateArea.Exchange;

internal class ExchangeView : IView<PresentExchangeRateResponse>
{
    public void Display(PresentExchangeRateResponse response)
    {
        if (response.DailyExchangeRates.Count == 0 || response.DailyExchangeRates.All(x => x.ExchangeRates.Count == 0))
            CustomConsole.WriteLineWarning($"There are no exchange rates.");
        else
            DisplayExchangeRates(response);

        if (response.Comments != null)
            CustomConsole.WriteLineWarning(response.Comments);
    }

    private static void DisplayExchangeRates(PresentExchangeRateResponse response)
    {
        ExchangeRatesDataGrid dataGrid = new(response);
        dataGrid.Display();
    }
}
