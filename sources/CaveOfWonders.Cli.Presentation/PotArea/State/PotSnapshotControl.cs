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

using DustInTheWind.CaveOfWonders.Cli.Application;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.State;

internal class PotSnapshotControl
{
    public DateTime Date { get; set; }

    public List<PotInstanceViewModel> Values { get; set; }

    public CurrencyValue Total { get; set; }

    public void Display()
    {
        DataGrid dataGrid = DataGridTemplate.CreateNew();
        dataGrid.Title = $"State - {Date:d} ({Total.Currency})";

        AddColumns(dataGrid);
        AddRows(dataGrid);

        dataGrid.FooterRow.FooterCell.Content = $"Total: {Total.ToDisplayString()}";

        dataGrid.Display();
    }

    private static void AddColumns(DataGrid dataGrid)
    {
        dataGrid.Columns.Add(new Column("Id")
        {
            ForegroundColor = ConsoleColor.DarkGray
        });

        dataGrid.Columns.Add(new Column("Name"));

        dataGrid.Columns.Add(new Column("Value")
        {
            CellHorizontalAlignment = HorizontalAlignment.Right
        });

        dataGrid.Columns.Add(new Column("Date")
        {
            CellHorizontalAlignment = HorizontalAlignment.Right
        });

        dataGrid.Columns.Add(new Column("Normalized Value")
        {
            CellHorizontalAlignment = HorizontalAlignment.Right
        });
    }

    private void AddRows(DataGrid dataGrid)
    {
        IEnumerable<ContentRow> rows = Values
            .Select(CreateRow);

        dataGrid.Rows.AddRange(rows);
    }

    private static ContentRow CreateRow(PotInstanceViewModel potInstanceViewModel)
    {
        string id = potInstanceViewModel.Id.ToString("D")[..8];
        string name = potInstanceViewModel.Name;
        string value = potInstanceViewModel.OriginalValue?.ToDisplayString();
        string date = potInstanceViewModel.Date?.ToString("d");
        string normalizedValue = potInstanceViewModel.NormalizedValue?.ToDisplayString();

        ContentRow row = new(id, name, value, date, normalizedValue);

        if (potInstanceViewModel.IsPotActive)
        {
            if (!potInstanceViewModel.IsValueActual)
            {
                row[2].ForegroundColor = ConsoleColor.DarkYellow;
                row[3].ForegroundColor = ConsoleColor.DarkYellow;
            }

            if (!potInstanceViewModel.IsValueAlreadyNormal)
                row[2].ForegroundColor = ConsoleColor.DarkGray;
        }
        else
        {
            row.ForegroundColor = ConsoleColor.DarkGray;
        }

        if (potInstanceViewModel.NormalizedValue?.Value != 0 && !potInstanceViewModel.IsValueAlreadyNormal && !potInstanceViewModel.IsNormalizedCurrent)
            row[4].ForegroundColor = ConsoleColor.DarkYellow;

        return row;
    }
}