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

using DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.State;
using DustInTheWind.ConsoleTools.Controls;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.ExchangeRateArea.Convert;

internal class ExchangeRatesControl : BlockControl
{
    public List<ExchangeRateViewModel> ExchangeRates { get; set; }

    protected override void DoDisplayContent(ControlDisplay display)
    {
        display.WriteRow("Conversion Rates:");

        foreach (ExchangeRateViewModel exchangeRate in ExchangeRates)
        {
            display.StartRow();
            display.Write($"  1 {exchangeRate.SourceCurrency} = {exchangeRate.Value:N4} {exchangeRate.DestinationCurrency}");

            if (!exchangeRate.IsCurrent)
                display.Write(ConsoleColor.DarkYellow, null, $" ({exchangeRate.CurrencyDate:d})");

            display.EndRow();
        }
    }
}