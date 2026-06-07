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

using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Controls;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.Controls;

internal class ExchangeRateControl : InlineControl
{
    public ExchangeRateViewModel ViewModel { get; set; }

    protected override void DoDisplayContent()
    {
        if (ViewModel == null)
            return;

        Console.Write($"1 {ViewModel.SourceCurrency} = {ViewModel.Value:N4} {ViewModel.DestinationCurrency}");

        if (!ViewModel.IsCurrent)
            CustomConsole.Write(ConsoleColor.DarkYellow, $" ({ViewModel.CurrencyDate:d})");
    }
}