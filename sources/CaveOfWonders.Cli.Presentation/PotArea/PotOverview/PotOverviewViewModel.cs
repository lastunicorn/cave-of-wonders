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

using System.Globalization;
using DustInTheWind.CaveOfWonders.Cli.Application;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentPots;
using DustInTheWind.CaveOfWonders.Cli.Presentation.Controls;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.PotOverview;

internal class PotOverviewViewModel
{
    public CultureInfo Culture { get; set; }

    public DateOnly Date { get; }

    public List<PotSnapshotViewModel> Values { get; }

    public List<ExchangeRateViewModel> ConversionRates { get; }

    public CurrencyValue Total { get; }

    public List<CurrencyTotalOverview> CurrencyTotalOverviews { get; }

    public PotOverviewViewModel(PresentPotsResponse presentPotsResponse)
    {
        Date = presentPotsResponse.Date;

        Values = presentPotsResponse.PotInstances
            .Select(x => new PotSnapshotViewModel
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