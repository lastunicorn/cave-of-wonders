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
using DustInTheWind.CaveOfWonders.Cli.Application.PresentPots;
using DustInTheWind.CaveOfWonders.Cli.Presentation.Controls;
using System.Globalization;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Pots;

internal class PotsViewModel
{
    public CultureInfo Culture { get; set; }

    public DateTime Date { get; }

    public List<PotInstanceViewModel> Values { get; }

    public List<ExchangeRateViewModel> ConversionRates { get; }

    public CurrencyValue Total { get; }

    public List<CurrencyTotalOverview> CurrencyTotalOverviews { get; }

    public PotsViewModel(PresentPotsResponse presentPotsResponse)
    {
        Date = presentPotsResponse.Date;

        Values = presentPotsResponse.PotInstances
            .Select(x => new PotInstanceViewModel
            {
                Id = x.Id,
                Name = x.Name,
                OriginalValue = x.IsActive
                    ? x.Value
                    : null,
                IsValueActual = x.Value?.Date == presentPotsResponse.Date,
                IsValueAlreadyNormal = x.Value?.Currency == x.NormalizedValue?.Currency,
                IsNormalizedCurrent = x.NormalizedValue?.Date == Date,
                Date = x.IsActive
                    ? x.Value?.Date
                    : null,
                NormalizedValue = x.NormalizedValue,
                IsPotActive = x.IsActive
            })
            .ToList();

        ConversionRates = presentPotsResponse.ConversionRates
            .Select(x => new ExchangeRateViewModel(x, x.Date == presentPotsResponse.Date))
            .ToList();

        Total = presentPotsResponse.Total;
        CurrencyTotalOverviews = presentPotsResponse.CurrencyTotalOverviews;
    }
}