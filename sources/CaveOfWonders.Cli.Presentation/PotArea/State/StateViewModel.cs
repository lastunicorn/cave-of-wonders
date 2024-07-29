// Cave of Wonders
// Copyright (C) 2023-2024 Dust in the Wind
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
using DustInTheWind.CaveOfWonders.Cli.Application.PresentState;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.State;

internal class StateViewModel
{
    public DateTime Date { get; set; }

    public List<PotInstanceViewModel> Values { get; set; }

    public List<ExchangeRateViewModel> ConversionRates { get; set; }

    public CurrencyValue Total { get; set; }

    public StateViewModel(PresentStateResponse presentStateResponse)
    {
        Date = presentStateResponse.Date;

        Values = presentStateResponse.PotInstances
            .Select(x => new PotInstanceViewModel
            {
                Id = x.Id,
                Name = x.Name,
                OriginalValue = x.IsActive
                    ? x.OriginalValue
                    : null,
                IsValueActual = x.Date == presentStateResponse.Date,
                IsValueAlreadyNormal = x.OriginalValue?.Currency == x.NormalizedValue?.Currency,
                Date = x.IsActive
                    ? x.Date
                    : null,
                NormalizedValue = x.NormalizedValue,
                IsPotActive = x.IsActive
            })
            .ToList();

        ConversionRates = presentStateResponse.ConversionRates
            .Select(x => new ExchangeRateViewModel(x))
            .ToList();

        Total = presentStateResponse.Total;
    }
}