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

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.WageArea.WageImport;

internal class WageImportView : IView<WageImportViewModel>
{
    public void Display(WageImportViewModel viewModel)
    {
        DataGrid dataGrid = new DataGrid();

        dataGrid.Columns.Add("Name");
        dataGrid.Columns.Add("Value", HorizontalAlignment.Right);

        dataGrid.Rows.Add("Total records", viewModel.TotalCount);
        dataGrid.Rows.Add("Added records", viewModel.AddedCount);
        dataGrid.Rows.Add("Updated records", viewModel.UpdatedCount);
        dataGrid.Rows.Add("Deleted records", viewModel.DeletedCount);

        dataGrid.Display();
    }
}