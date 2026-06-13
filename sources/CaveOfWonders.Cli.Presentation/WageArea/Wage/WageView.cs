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

using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.WageArea.Wage;

internal class WageView : ViewBase<WagesViewModel>
{
    public override void Display(WagesViewModel viewModel)
    {
        DataGrid dataGrid = new() { Title = "Average Wage" };

        dataGrid.Columns.Add("Year");
        dataGrid.Columns.Add("Gross", HorizontalAlignment.Right);
        dataGrid.Columns.Add("Net", HorizontalAlignment.Right);

        foreach (WageViewModel viewModelWage in viewModel.Wages.OrderBy(x => x.Year))
        {
            int year = viewModelWage.Year;
            string gross = viewModelWage.GrossValue.HasValue
                ? viewModelWage.GrossValue.Value.ToString("N2")
                : string.Empty;
            string net = viewModelWage.NetValue.HasValue
                ? viewModelWage.NetValue.Value.ToString("N2")
                : string.Empty;

            dataGrid.Rows.Add(year, gross, net);
        }

        dataGrid.Display();
    }
}