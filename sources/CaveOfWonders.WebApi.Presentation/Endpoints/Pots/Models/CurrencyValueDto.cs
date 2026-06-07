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

using DustInTheWind.CaveOfWonders.Cli.Application;

namespace CaveOfWonders.WebApi.Presentation.Endpoints.Pots.Models;

/// <summary>
/// Represents a monetary value with its currency and date
/// </summary>
public class CurrencyValueDto
{
    /// <summary>
    /// The monetary value
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// The currency code (e.g., USD, EUR, RON)
    /// </summary>
    public string Currency { get; set; }

    /// <summary>
    /// The date when this value was recorded or is valid
    /// </summary>
    public DateTime Date { get; set; }

    internal static CurrencyValueDto From(CurrencyValue currencyValue)
    {
        if (currencyValue == null)
            return null;

        return new CurrencyValueDto
        {
            Value = currencyValue.Value,
            Currency = currencyValue.Currency,
            Date = currencyValue.Date
        };
    }
}