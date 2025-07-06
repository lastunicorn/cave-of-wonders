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

using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportExchangeRates;

public class ImportExchangeRatesResponse
{
    public int TotalCount { get; set; }

    public int AddedCount { get; set; }

    public int ExistingUpdatedCount { get; set; }

    public int ExistingIdenticalCount { get; set; }

    public int NewDuplicateIdenticalCount { get; set; }

    public int NewDuplicateDifferentCount { get; set; }

    public List<UpdateReportResponseDto> Updates { get; } = new();

    public List<DuplicateReportResponseDto> Duplicates { get; } = new();

    internal ImportExchangeRatesResponse(ExchangeRateImportReport report)
    {
        TotalCount = report.TotalCount;
        AddedCount = report.AddedCount;
        ExistingUpdatedCount = report.ExistingUpdatedCount;
        ExistingIdenticalCount = report.ExistingIdenticalCount;
        NewDuplicateIdenticalCount = report.NewDuplicateIdenticalCount;
        NewDuplicateDifferentCount = report.NewDuplicateDifferentCount;
        Updates.AddRange(report.Updates.Select(x => new UpdateReportResponseDto(x)));
        Duplicates.AddRange(report.Duplicates.Select(x => new DuplicateReportResponseDto(x)));
    }
}