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

using DustInTheWind.CaveOfWonders.Cli.Application.PresentPots;

namespace CaveOfWonders.WebApi.Presentation.Controllers.Pot.Models;

/// <summary>
/// Overview of totals for a specific currency
/// </summary>
public class CurrencyTotalOverviewDto
{
    /// <summary>
    /// Total value in the original currency
    /// </summary>
    public CurrencyValueDto Value { get; set; }

    /// <summary>
    /// Total value normalized to a common currency
    /// </summary>
    public CurrencyValueDto NormalizedValue { get; set; }

    /// <summary>
    /// Percentage of this currency relative to the total portfolio
    /// </summary>
    public decimal Percentage { get; set; }

    internal static CurrencyTotalOverviewDto From(CurrencyTotalOverview currencyTotalOverview)
    {
        if (currencyTotalOverview == null)
            return null;

        return new CurrencyTotalOverviewDto
        {
            Value = CurrencyValueDto.From(currencyTotalOverview.Value),
            NormalizedValue = CurrencyValueDto.From(currencyTotalOverview.NormalizedValue),
            Percentage = currencyTotalOverview.Percentage
        };
    }
}