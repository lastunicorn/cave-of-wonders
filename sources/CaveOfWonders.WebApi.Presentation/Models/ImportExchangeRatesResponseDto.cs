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

using DustInTheWind.CaveOfWonders.Cli.Application.ImportExchangeRates;

namespace CaveOfWonders.WebApi.Presentation.Models;

/// <summary>
/// Response model for the import exchange rates operation
/// </summary>
public class ImportExchangeRatesResponseDto
{
    /// <summary>
    /// Total number of exchange rate records processed
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Number of new exchange rate records added
    /// </summary>
    public int AddedCount { get; set; }

    /// <summary>
    /// Number of existing exchange rate records updated
    /// </summary>
    public int ExistingUpdatedCount { get; set; }

    /// <summary>
    /// Number of existing exchange rate records that were identical (not updated)
    /// </summary>
    public int ExistingIdenticalCount { get; set; }

    /// <summary>
    /// Number of new duplicate records that were identical
    /// </summary>
    public int NewDuplicateIdenticalCount { get; set; }

    /// <summary>
    /// Number of new duplicate records that had different values
    /// </summary>
    public int NewDuplicateDifferentCount { get; set; }

    /// <summary>
    /// List of exchange rate records that were updated
    /// </summary>
    public List<UpdateReportDto> Updates { get; set; } = new();

    /// <summary>
    /// List of duplicate exchange rate records found
    /// </summary>
    public List<DuplicateReportDto> Duplicates { get; set; } = new();

    /// <summary>
    /// Creates a response DTO from the application response
    /// </summary>
    public static ImportExchangeRatesResponseDto FromApplication(ImportExchangeRatesResponse response)
    {
        return new ImportExchangeRatesResponseDto
        {
            TotalCount = response.TotalCount,
            AddedCount = response.AddedCount,
            ExistingUpdatedCount = response.ExistingUpdatedCount,
            ExistingIdenticalCount = response.ExistingIdenticalCount,
            NewDuplicateIdenticalCount = response.NewDuplicateIdenticalCount,
            NewDuplicateDifferentCount = response.NewDuplicateDifferentCount,
            Updates = response.Updates
                .Select(update => new UpdateReportDto
                {
                    Date = update.Date,
                    CurrencyPair = update.CurrencyPair,
                    OldValue = update.OldValue,
                    NewValue = update.NewValue
                })
                .ToList(),
            Duplicates = response.Duplicates
                .Select(duplicate => new DuplicateReportDto
                {
                    Date = duplicate.Date,
                    CurrencyPair = duplicate.CurrencyPair,
                    Value1 = duplicate.Value1,
                    Value2 = duplicate.Value2
                })
                .ToList()
        };
    }
}