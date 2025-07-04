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

using DustInTheWind.CaveOfWonders.Cli.Application.PresentInflation;

namespace CaveOfWonders.WebApi.Presentation.Models
{
    /// <summary>
    /// Response DTO containing inflation records
    /// </summary>
    public class InflationResponseDto
    {
        /// <summary>
        /// List of inflation records
        /// </summary>
        public List<InflationRecordDto> InflationRecords { get; set; } = new List<InflationRecordDto>();

        /// <summary>
        /// Creates a response DTO from the application response
        /// </summary>
        public static InflationResponseDto FromApplicationResponse(PresentInflationResponse response)
        {
            InflationResponseDto dto = new();
            
            foreach (var record in response.InflationRecords)
            {
                dto.InflationRecords.Add(new InflationRecordDto
                {
                    Year = record.Year,
                    Value = record.Value
                });
            }

            return dto;
        }
    }
}