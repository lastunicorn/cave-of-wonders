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
using DustInTheWind.CaveOfWonders.Cli.Application.PresentState;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.State;

internal class PotSnapshotControl
{
    public DateTime Date { get; set; }

    public List<PotInstanceInfo> Values { get; set; }

    public CurrencyValue Total { get; set; }

    public void Display()
    {
        DataGrid dataGrid = DataGridTemplate.CreateNew();
        dataGrid.Title = $"{Date:d} ({Total.Currency})";

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

        dataGrid.Columns.Add("Name");

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
            .Select(x =>
            {
                string id = x.Id.ToString("D")[..8];
                string name = x.Name;
                string value = x.IsActive
                    ? x.OriginalValue?.ToDisplayString()
                    : string.Empty;
                string date = x.IsActive
                    ? x.Date.ToString("d")
                    : string.Empty;
                string normalizedValue = x.NormalizedValue?.ToDisplayString();

                ContentRow row = new(id, name, value, date, normalizedValue);

                bool valueIsActual = x.Date == Date;

                if (x.IsActive)
                {
                    if (!valueIsActual)
                    {
                        row[2].ForegroundColor = ConsoleColor.DarkYellow;
                        row[3].ForegroundColor = ConsoleColor.DarkYellow;
                    }

                    bool valueIsNormal = x.OriginalValue?.Currency == Total.Currency;

                    if (!valueIsNormal)
                        row[2].ForegroundColor = ConsoleColor.DarkGray;
                }
                else
                {
                    row.ForegroundColor = ConsoleColor.DarkGray;
                }

                return row;
            });

        dataGrid.Rows.AddRange(rows);
    }
}