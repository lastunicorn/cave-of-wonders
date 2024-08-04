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

using DustInTheWind.CaveOfWonders.Cli.Application.PresentInflation;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.InflationArea.Inflation;

internal class InflationView : IView<InflationViewModel>
{
    public void Display(InflationViewModel viewModel)
    {
        if (viewModel.Records?.Count > 0)
        {
            foreach (InflationRecord inflationRecord in viewModel.Records)
            {
                Console.Write($"{inflationRecord.Year}: {inflationRecord.Value,6:N2} ");
                DisplayChartLine(inflationRecord.Value);

                Console.WriteLine();
            }
        }
        else
        {
            CustomConsole.WriteLineWarning("No inflation rates.");
        }
    }

    private static void DisplayChartLine(decimal roundedValue)
    {
        ChartLineControl chartLineControl = new()
        {
            Value = roundedValue
        };
        chartLineControl.Display();
    }
}