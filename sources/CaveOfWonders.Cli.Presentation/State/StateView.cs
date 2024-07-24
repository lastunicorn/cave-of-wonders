// Cave of Wonders
// Copyright (C) 2023 Dust in the Wind
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

using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.State;

internal class StateView : IView<StateViewModel>
{
    public void Display(StateViewModel stateViewModel)
    {
        DisplayCaveInstances(stateViewModel);
        DisplayConversionRates(stateViewModel);
    }

    private void DisplayCaveInstances(StateViewModel stateViewModel)
    {
        DataGrid dataGrid = new()
        {
            TitleRow =
            {
                TitleCell =
                {
                    Content = $"{stateViewModel.Date:d} ({stateViewModel.Total.Currency})"
                },
                BackgroundColor = ConsoleColor.DarkGray,
                ForegroundColor = ConsoleColor.Black
            }
        };

        dataGrid.Columns.Add("Name");
        dataGrid.Columns.Add(new Column("Value")
        {
            CellHorizontalAlignment = HorizontalAlignment.Right
        });
        dataGrid.Columns.Add(new Column("Normalized Value")
        {
            CellHorizontalAlignment = HorizontalAlignment.Right
        });

        IEnumerable<ContentRow> rows = stateViewModel.Values
            .Select(x =>
            {
                string name = x.Name;
                string value = x.OriginalValue?.ToDisplayString();
                string normalizedValue = x.ConvertedValue?.ToDisplayString();

                return new ContentRow(name, value, normalizedValue);
            });

        dataGrid.Rows.AddRange(rows);

        dataGrid.FooterRow.FooterCell.Content = $"Total: {stateViewModel.Total.ToDisplayString()}";

        dataGrid.Display();
    }

    private void DisplayConversionRates(StateViewModel stateViewModel)
    {
        Console.WriteLine();
        Console.WriteLine("Conversion Rates:");

        foreach (ConversionRateViewModel conversionRate in stateViewModel.ConversionRates)
            Console.WriteLine($"\t{conversionRate}");
    }
}