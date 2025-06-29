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

using System.ComponentModel.DataAnnotations;
using DustInTheWind.CaveOfWonders.Cli.Application.ImportInflation;

namespace CaveOfWonders.WebApi.Presentation.Models;

/// <summary>
/// DTO for importing inflation data via API
/// </summary>
public class ImportInflationDto
{
    /// <summary>
    /// Path to the inflation source file (required when ImportSource is File)
    /// </summary>
    public string SourceFilePath { get; set; }

    /// <summary>
    /// Source of inflation data: 0 = File, 1 = Web
    /// </summary>
    [Required]
    public InflationImportSourceDto ImportSource { get; set; }

    /// <summary>
    /// Converts the DTO to the application request
    /// </summary>
    internal ImportInflationRequest ToApplicationRequest()
    {
        return new ImportInflationRequest
        {
            SourceFilePath = SourceFilePath,
            ImportSource = ImportSource == InflationImportSourceDto.File 
                ? DustInTheWind.CaveOfWonders.Cli.Application.ImportInflation.ImportSource.File 
                : DustInTheWind.CaveOfWonders.Cli.Application.ImportInflation.ImportSource.Web
        };
    }
}
