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

namespace CaveOfWonders.WebApi.Presentation.Endpoints.Pots.Models;

/// <summary>
/// Response containing all financial pots with their values and metadata
/// </summary>
public class GetSummaryPotsResponseDto
{
    /// <summary>
    /// Date for which the pot values are calculated
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// List of pot instances with their current values
    /// </summary>
    public List<PotInstanceInfoDto> PotInstances { get; set; }

    /// <summary>
    /// Exchange rates used for currency conversion
    /// </summary>
    public List<ExchangeRateInfoDto> ConversionRates { get; set; }

    /// <summary>
    /// Total value of all pots combined
    /// </summary>
    public CurrencyValueDto Total { get; set; }

    /// <summary>
    /// Overview of totals grouped by currency
    /// </summary>
    public List<CurrencyTotalOverviewDto> CurrencyTotalOverviews { get; set; }

    internal static GetSummaryPotsResponseDto From(PresentPotsResponse response)
    {
        return new GetSummaryPotsResponseDto
        {
            Date = response.Date,
            PotInstances = response.PotInstances?
                .Select(PotInstanceInfoDto.From)
                .ToList()
                ?? [],
            ConversionRates = response.ConversionRates?
                .Select(ExchangeRateInfoDto.From)
                .ToList()
                ?? [],
            Total = CurrencyValueDto.From(response.Total),
            CurrencyTotalOverviews = response.CurrencyTotalOverviews?
                .Select(CurrencyTotalOverviewDto.From)
                .ToList()
                ?? []
        };
    }
}