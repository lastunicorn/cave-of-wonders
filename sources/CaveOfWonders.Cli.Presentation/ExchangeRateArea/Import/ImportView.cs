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

using System.Data;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.ExchangeRateArea.Import;

internal class ImportView : IView<ImportResultViewModel>
{
    public void Display(ImportResultViewModel result)
    {
        foreach (UpdateValueViewModel updateValueViewModel in result.Updates)
        {
            Console.WriteLine();
            Console.WriteLine($"Existing : {updateValueViewModel.Date} {updateValueViewModel.CurrencyPair} {updateValueViewModel.OldValue}");
            Console.WriteLine($"New      : {updateValueViewModel.Date} {updateValueViewModel.CurrencyPair} {updateValueViewModel.NewValue}");
        }

        foreach (DuplicateValueViewModel duplicateValueViewModel in result.Duplicates)
        {
            Console.WriteLine();
            Console.WriteLine($"Duplicate 1 : {duplicateValueViewModel.Date} {duplicateValueViewModel.CurrencyPair} {duplicateValueViewModel.Value1}");
            Console.WriteLine($"Duplicate 2 : {duplicateValueViewModel.Date} {duplicateValueViewModel.CurrencyPair} {duplicateValueViewModel.Value2}");
        }

        Console.WriteLine();

        DataGrid dataGrid = DataGridTemplate.CreateNew();
        dataGrid.Title = "Import Report";
        dataGrid.DisplayBorderBetweenRows = true;
        //dataGrid.HeaderRow.BackgroundColor = ConsoleColor.Gray;
        //dataGrid.HeaderRow.ForegroundColor = ConsoleColor.Black;

        dataGrid.Columns.Add(new Column("Name")
        {
            ForegroundColor = ConsoleColor.White
        });
        dataGrid.Columns.Add(new Column("Value")
        {
            CellHorizontalAlignment = HorizontalAlignment.Right
        });
        dataGrid.Columns.Add(new Column("Comments")
        {
            ForegroundColor = ConsoleColor.DarkGray
        });

        dataGrid.HeaderRow.IsVisible = false;

        ContentRow contentRowAdded = dataGrid.Rows.Add("Added", result.AddedCount.ToStringOrEmpty("-"), "New items that were added into the storage.");
        if (result.AddedCount > 0)
            contentRowAdded[1].ForegroundColor = ConsoleColor.Green;

        ContentRow contentRowUpdated = dataGrid.Rows.Add("Updated", result.ExistingUpdatedCount.ToStringOrEmpty("-"), "The items already exist in the storage, but with different values.\nValues were updated.");
        if (result.ExistingUpdatedCount > 0)
            contentRowUpdated[1].ForegroundColor = ConsoleColor.Green;

        dataGrid.Rows.Add("Existing", result.ExistingIdenticalCount.ToStringOrEmpty("-"), "The items already exist in the storage.\nNothing to do.");

        if (result.NewDuplicateIdenticalCount > 0)
        {
            ContentRow contentRowSourceDuplicatesIdentical = dataGrid.Rows.Add("Source Duplicates\n(identical)", result.NewDuplicateIdenticalCount.ToStringOrEmpty("-"), "The import source provided same exchange rate multiple times.\nDuplicates were ignored.");
            contentRowSourceDuplicatesIdentical[1].ForegroundColor = ConsoleColor.DarkRed;
        }

        if (result.NewDuplicateIdenticalCount > 0)
        {
            ContentRow contentRowSourceDuplicatesDifferent = dataGrid.Rows.Add("Source Duplicates\n(different value)", result.NewDuplicateDifferentCount.ToStringOrEmpty("-"), "The import source provided same currency multiple times, but with different values.\nDuplicates were ignored.");
            contentRowSourceDuplicatesDifferent[1].ForegroundColor = ConsoleColor.DarkRed;
        }

        dataGrid.Display();
    }
}