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

using DustInTheWind.CaveOfWonders.Cli.Application.PresentCpi;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.CpiArea.Cpi;

internal class CpiView : IView<CpiViewModel>
{
    public void Display(CpiViewModel viewModel)
    {
        if (viewModel.Records?.Count > 0)
        {
            DataGrid dataGrid = new();

            dataGrid.Columns.Add("Year");
            dataGrid.Columns.Add("Value %", HorizontalAlignment.Right);

            foreach (CpiDto cpiDto in viewModel.Records)
            {
                dataGrid.Rows.Add(cpiDto.Year, cpiDto.Value);
            }

            dataGrid.Display();
        }
        else
        {
            CustomConsole.WriteLineWarning("No inflation rates.");
        }
    }
}