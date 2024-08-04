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

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.InflationArea.Inflation;

internal class ChartLineControl
{
    private const int Threshold = 3;

    public decimal Value { get; set; }

    public void Display()
    {
        if (Value <= 0)
            return;

        int roundedValue = (int)Math.Round(Math.Max(0, Value));

        int safeValue = Math.Min(Threshold, roundedValue);
        DisplayValue(safeValue, ConsoleColor.DarkGray);

        int painfulValue = roundedValue - safeValue;
        if (painfulValue > 0)
            DisplayValue(painfulValue, ConsoleColor.White);
    }

    private static void DisplayValue(int value, ConsoleColor color)
    {
        string chartLine = new('.', value);
        CustomConsole.Write(color, chartLine);
    }
}