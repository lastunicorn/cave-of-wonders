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

using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation;

internal static class DataGridTemplate
{
    public static DataGrid CreateNew()
    {
        return new DataGrid
        {
            TitleRow =
            {
                BackgroundColor = ConsoleColor.Gray,
                ForegroundColor = ConsoleColor.Black
            }
        };
    }

    public static DataGrid Disable(DataGrid dataGrid)
    {
        dataGrid.TitleRow.BackgroundColor = ConsoleColor.DarkGray;
        dataGrid.Border.ForegroundColor = ConsoleColor.DarkGray;
        dataGrid.ForegroundColor = ConsoleColor.DarkGray;

        return dataGrid;
    }
}