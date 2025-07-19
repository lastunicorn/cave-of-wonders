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

using DustInTheWind.CaveOfWonders.Cli.Application.Convert;
using DustInTheWind.CaveOfWonders.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CaveOfWonders.WebApi.Presentation.Controllers.ExchangeRate.Models;


/// <summary>
/// Request model for currency conversion
/// </summary>
public class ExchangeRateConvertRequestDto
{
    /// <summary>
    /// Value to be converted
    /// </summary>
    [FromQuery]
    [Required]
    public decimal Value { get; set; }

    /// <summary>
    /// Source currency (3-letter code)
    /// </summary>
    [FromQuery]
    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string SourceCurrency { get; set; } = string.Empty;

    /// <summary>
    /// Destination currency (3-letter code)
    /// </summary>
    [FromQuery]
    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string DestinationCurrency { get; set; } = string.Empty;

    /// <summary>
    /// Date of the exchange rate to use (defaults to latest available)
    /// </summary>
    [FromQuery]
    public DateTime? Date { get; set; }

    /// <summary>
    /// Converts the DTO to the application request
    /// </summary>
    public ConvertRequest ToApplication()
    {
        return new ConvertRequest
        {
            InitialValue = Value,
            CurrencyPair = new CurrencyPair
            {
                Currency1 = SourceCurrency,
                Currency2 = DestinationCurrency
            },
            Date = Date
        };
    }
}