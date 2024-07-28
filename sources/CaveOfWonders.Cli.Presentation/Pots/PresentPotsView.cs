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

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.Pots;

internal class PresentPotsView : IView<PresentPotsViewModel>
{
    public void Display(PresentPotsViewModel viewModel)
    {
        DataGrid dataGrid = new()
        {
            TitleRow =
            {
                TitleCell =
                {
                    Content = $"Pots ({viewModel.Date:d})"
                },
                BackgroundColor = ConsoleColor.Gray,
                ForegroundColor = ConsoleColor.Black
            }
        };

        dataGrid.Columns.Add(new Column("Id")
        {
            ForegroundColor = ConsoleColor.DarkGray
        });
        dataGrid.Columns.Add(new Column("Name"));
        dataGrid.Columns.Add(new Column("Value")
        {
            CellHorizontalAlignment = HorizontalAlignment.Right
        });

        IEnumerable<ContentRow> rows = viewModel.Pots
            .Select(x =>
            {
                string id = x.Id.ToString("D")[..8];
                string name = x.Name;
                string value = x.Value.ToDisplayString();

                ContentRow contentRow = new(id, name, value);

                if (!x.IsActive)
                    contentRow.ForegroundColor = ConsoleColor.DarkGray;

                return contentRow;
            });

        dataGrid.Rows.AddRange(rows);

        dataGrid.Display();
    }
}