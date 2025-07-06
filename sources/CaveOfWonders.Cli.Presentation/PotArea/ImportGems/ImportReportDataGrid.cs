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

using DustInTheWind.CaveOfWonders.Cli.Application.ImportGems.Importing;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.ImportGems;

internal class ImportReportDataGrid : DataGrid
{
    public ImportReportDataGrid()
    {
        _ = Columns.Add(new Column("Id")
        {
            ForegroundColor = ConsoleColor.DarkGray
        });

        _ = Columns.Add("Pot");

        _ = Columns.Add(new Column("Added")
        {
            CellHorizontalAlignment = DustInTheWind.ConsoleTools.Controls.HorizontalAlignment.Right,
            ForegroundColor = ConsoleColor.Green
        });

        _ = Columns.Add(new Column("Skipped\n(exists)")
        {
            CellHorizontalAlignment = DustInTheWind.ConsoleTools.Controls.HorizontalAlignment.Right
        });

        _ = Columns.Add(new Column("Skipped\n(pot not active)")
        {
            CellHorizontalAlignment = DustInTheWind.ConsoleTools.Controls.HorizontalAlignment.Right,
            ForegroundColor = ConsoleColor.DarkYellow
        });
    }

    public void AddRows(IEnumerable<PotImportReport> report)
    {
        IEnumerable<ContentRow> rows = report.Select(CreateRow);
        Rows.AddRange(rows);
    }

    private static ContentRow CreateRow(PotImportReport potImportReport)
    {
        string id = potImportReport.PotId.ToString("N")[..8];

        string potName = potImportReport.PotName;

        string addedCount = potImportReport.AddCount == 0
            ? string.Empty
            : potImportReport.AddCount.ToString();

        string skippedCount = potImportReport.SkipExistsCount == 0
            ? string.Empty
            : potImportReport.SkipExistsCount.ToString();

        string skippedNotActiveCount = potImportReport.SkipNotActiveCount == 0
            ? string.Empty
            : potImportReport.SkipNotActiveCount.ToString();

        ContentRow contentRow = new(id, potName, addedCount, skippedCount, skippedNotActiveCount);

        if (potName == null)
            contentRow.ForegroundColor = ConsoleColor.DarkRed;

        return contentRow;
    }
}