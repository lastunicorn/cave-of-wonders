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
        Console.WriteLine($"Import Count: {result.TotalCount}");
        Console.WriteLine($"Added: {result.AddedCount}; Updated: {result.ExistingUpdatedCount}; Existing: {result.ExistingIdenticalCount}");
        Console.WriteLine($"Input Duplicates (identical): {result.NewDuplicateIdenticalCount}");
        Console.WriteLine($"Input Duplicates (different value): {result.NewDuplicateDifferentCount}");
    }
}