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

using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.SystemAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentState;

public class PresentStateUseCase : IRequestHandler<PresentStateRequest, PresentStateResponse>
{
    private readonly ISystemClock systemClock;
    private readonly IUnitOfWork unitOfWork;

    public PresentStateUseCase(ISystemClock systemClock, IUnitOfWork unitOfWork)
    {
        this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<PresentStateResponse> Handle(PresentStateRequest request, CancellationToken cancellationToken)
    {
        DateTime date = request.Date ?? systemClock.Today;
        string currency = request.Currency ?? "RON";

        List<ConversionRate> conversionRates = await RetrieveConversionRates(date);
        List<PotInstanceInfo> potInstances = await RetrievePotInstances(date, currency, conversionRates, request.IncludeInactive);

        return new PresentStateResponse
        {
            Date = date,
            PotInstances = potInstances,
            ConversionRates = conversionRates
                .Select(x => new ConversionRateInfo(x))
                .ToList(),
            Total = new CurrencyValue
            {
                Value = potInstances.Sum(x => x.NormalizedValue?.Value ?? x.OriginalValue?.Value ?? 0),
                Currency = currency
            }
        };
    }

    private async Task<List<ConversionRate>> RetrieveConversionRates(DateTime date)
    {
        IEnumerable<ConversionRate> conversionRates = await unitOfWork.ConversionRateRepository.GetAll(date);
        return conversionRates.ToList();
    }

    private async Task<List<PotInstanceInfo>> RetrievePotInstances(DateTime date, string currency, List<ConversionRate> conversionRates, bool includeInactive)
    {
        IEnumerable<PotInstance> potInstances = await unitOfWork.PotRepository.GetInstances(date, DateMatchingMode.LastAvailable, includeInactive);
        potInstances = potInstances.OrderBy(x => x.Pot.DisplayOrder);

        PotInstanceTransformation potInstanceTransformation = new()
        {
            Date = date,
            Instances = potInstances.ToList(),
            DestinationCurrency = currency,
            ConversionRates = conversionRates
        };

        return potInstanceTransformation.Transform().ToList();
    }
}