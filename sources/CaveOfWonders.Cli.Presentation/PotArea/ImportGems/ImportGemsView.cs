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
using DustInTheWind.CsvParser.Application.Importing;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.ImportGems;

internal class ImportGemsView : IView<ImportGemsViewModel>
{
    public void Display(ImportGemsViewModel viewModel)
    {
        DisplayReports(viewModel.Report);
    }

    private static void DisplayReports(IEnumerable<PotImportReport> report)
    {
        DataGrid dataGrid = DataGridTemplate.CreateNew();

        dataGrid.Columns.Add(new Column("Id")
        {
            ForegroundColor = ConsoleColor.DarkGray
        });
        dataGrid.Columns.Add("Pot");
        dataGrid.Columns.Add(new Column("Added")
        {
            CellHorizontalAlignment = HorizontalAlignment.Right
        });
        dataGrid.Columns.Add(new Column("Skipped")
        {
            CellHorizontalAlignment = HorizontalAlignment.Right
        });

        IEnumerable<ContentRow> rows = report
            .Select(CreateRow);

        dataGrid.Rows.AddRange(rows);

        dataGrid.Display();
    }

    private static ContentRow CreateRow(PotImportReport potImportReport)
    {
        string id = potImportReport.PotId.ToString("N")[..8];
        string potName = potImportReport.PotName;
        string addedCount = potImportReport.AddCount == 0
            ? string.Empty
            : potImportReport.AddCount.ToString();
        string skippedCount = potImportReport.SkipCount == 0
            ? string.Empty
            : potImportReport.SkipCount.ToString();

        return new ContentRow(id, potName, addedCount, skippedCount);
    }
}