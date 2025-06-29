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

using DustInTheWind.CaveOfWonders.Cli.Application;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Pots;

internal class TotalsDataGrid
{
    public DateTime Date { get; set; }

    public List<CurrencyValue> Values { get; set; }

    public CurrencyValue Total { get; set; }

    public void Display()
    {
        DataGrid dataGrid = DataGridTemplate.CreateNew();
        dataGrid.Title = $"Totals - {Date:d} ({Total.Currency})";

        AddColumns(dataGrid);
        AddRows(dataGrid);
        AddFooter(dataGrid);

        dataGrid.Display();
    }

    private static void AddColumns(DataGrid dataGrid)
    {
        dataGrid.Columns.Add(new Column("Value"));

        dataGrid.Columns.Add(new Column("Normalized Value")
        {
            CellHorizontalAlignment = HorizontalAlignment.Right
        });

        dataGrid.Columns.Add(new Column("Percentage")
        {
            CellHorizontalAlignment = HorizontalAlignment.Right
        });
    }

    private void AddRows(DataGrid dataGrid)
    {
        IEnumerable<ContentRow> rows = Values
            .Select(static x =>
            {
                ContentRow row = new();

                string value = x.ToDisplayString();
                ContentCell valueCell = row.AddCell(value);
                valueCell.HorizontalAlignment = HorizontalAlignment.Right;

                return row;
            });

        dataGrid.Rows.AddRange(rows);
    }

    private void AddFooter(DataGrid dataGrid)
    {
        string totalText = $"Total: {Total.ToDisplayString()}";

        dataGrid.Footer = totalText;
        dataGrid.FooterRow.ForegroundColor = ConsoleColor.White;
    }
}