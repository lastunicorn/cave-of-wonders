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

using DustInTheWind.CaveOfWonders.Cli.Application.ImportFromBnrFile;
using DustInTheWind.CaveOfWonders.Cli.Application.ImportFromNbrFile;
using DustInTheWind.ConsoleTools.Commando;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.ImportNbr;

internal class ImportNbrView : IView<ImportFromNbrResponse>
{
    public void Display(ImportFromNbrResponse response)
    {
        foreach (UpdateReportResponseDto updateReportResponseDto in response.Updates)
        {
            Console.WriteLine();
            Console.WriteLine($"Existing : {updateReportResponseDto.Date} {updateReportResponseDto.CurrencyPair} {updateReportResponseDto.OldValue}");
            Console.WriteLine($"New      : {updateReportResponseDto.Date} {updateReportResponseDto.CurrencyPair} {updateReportResponseDto.NewValue}");
        }

        foreach (DuplicateReportResponseDto duplicateReportResponseDto in response.Duplicates)
        {
            Console.WriteLine();
            Console.WriteLine($"Duplicate 1 : {duplicateReportResponseDto.Date} {duplicateReportResponseDto.CurrencyPair} {duplicateReportResponseDto.Value1}");
            Console.WriteLine($"Duplicate 2 : {duplicateReportResponseDto.Date} {duplicateReportResponseDto.CurrencyPair} {duplicateReportResponseDto.Value2}");
        }

        Console.WriteLine();
        Console.WriteLine($"Import Count: {response.TotalCount}");
        Console.WriteLine($"Added: {response.AddedCount}; Updated: {response.ExistingUpdatedCount}; Existing: {response.ExistingIdenticalCount}");
        Console.WriteLine($"Input Duplicates (identical): {response.NewDuplicateIdenticalCount}");
        Console.WriteLine($"Input Duplicates (different value): {response.NewDuplicateDifferentCount}");
    }
}