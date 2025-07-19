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

using DustInTheWind.CaveOfWonders.Cli.Application.ImportInflation;

namespace CaveOfWonders.WebApi.Presentation.Controllers.Inflation.Models;

/// <summary>
/// Response DTO for inflation data import operation
/// </summary>
public class ImportInflationResponseDto
{
    /// <summary>
    /// Number of new inflation records added
    /// </summary>
    public int AddedCount { get; set; }

    /// <summary>
    /// Number of existing inflation records updated
    /// </summary>
    public int UpdatedCount { get; set; }

    /// <summary>
    /// Total number of records processed (added + updated)
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Creates a response DTO from the application response
    /// </summary>
    internal static ImportInflationResponseDto FromApplicationResponse(ImportInflationResponse response)
    {
        return new ImportInflationResponseDto
        {
            AddedCount = response.AddedCount,
            UpdatedCount = response.UpdatedCount,
            TotalCount = response.TotalCount
        };
    }
}