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
using DustInTheWind.CaveOfWonders.Infrastructure;

namespace CaveOfWonders.WebApi.Presentation.Models;

/// <summary>
/// Represents a currency pair for exchange rate operations
/// </summary>
public class CurrencyPairDto
{
    /// <summary>
    /// First currency in the pair (3-letter code)
    /// </summary>
    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Currency1 { get; set; } = string.Empty;

    /// <summary>
    /// Second currency in the pair (3-letter code)
    /// </summary>
    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Currency2 { get; set; } = string.Empty;

    /// <summary>
    /// Converts the DTO to the domain CurrencyPair
    /// </summary>
    public CurrencyPair ToDomain()
    {
        return new CurrencyPair
        {
            Currency1 = Currency1,
            Currency2 = Currency2
        };
    }

    /// <summary>
    /// Creates a DTO from a domain CurrencyPair
    /// </summary>
    public static CurrencyPairDto FromDomain(CurrencyPair currencyPair)
    {
        return new CurrencyPairDto
        {
            Currency1 = currencyPair.Currency1,
            Currency2 = currencyPair.Currency2
        };
    }
}