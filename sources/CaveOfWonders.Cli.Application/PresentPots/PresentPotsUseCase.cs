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

using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.SystemAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPots;

public class PresentPotsUseCase : IRequestHandler<PresentPotsRequest, PresentPotsResponse>
{
    private readonly ISystemClock systemClock;
    private readonly IUnitOfWork unitOfWork;
    private readonly CurrenciesConvertor currenciesConverter;

    public PresentPotsUseCase(ISystemClock systemClock, IUnitOfWork unitOfWork)
    {
        this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        currenciesConverter = new CurrenciesConvertor(unitOfWork);
    }

    public async Task<PresentPotsResponse> Handle(PresentPotsRequest request, CancellationToken cancellationToken)
    {
        DateOnly currentDate = request.Date ?? systemClock.Today;
        string defaultCurrency = request.Currency ?? "RON";

        IEnumerable<PotSnapshot> potSnapshots = await RetrievePotSnapshotsFromStorage(currentDate, request.IncludeInactive);
        List<PotInstanceInfo> potInstanceInfos = await ConvertToPotInstanceInfos(potSnapshots, currentDate, defaultCurrency);

        PotsAnalysis potsAnalysis = new(currenciesConverter)
        {
            PotInstanceInfos = potInstanceInfos,
            TargetDate = currentDate,
            TargetCurrency = defaultCurrency
        };

        await potsAnalysis.Calculate();

        return new PresentPotsResponse
        {
            Date = currentDate,
            PotInstances = potInstanceInfos,
            ConversionRates = currenciesConverter.UsedExchangeRates
                .Select(x => new ExchangeRateInfo(x))
                .ToList(),
            Total = new CurrencyValue
            {
                Value = potsAnalysis.TotalValue,
                Currency = defaultCurrency
            },
            CurrencyTotalOverviews = potsAnalysis.currencyTotalOverviews
        };
    }

    private async Task<IEnumerable<PotSnapshot>> RetrievePotSnapshotsFromStorage(DateOnly date, bool includeInactive)
    {
        IEnumerable<PotSnapshot> potSnapshots = await unitOfWork.PotRepository.GetSnapshots(date, DateMatchingMode.LastAvailable, includeInactive);
        return potSnapshots.OrderBy(x => x.Pot.DisplayOrder);
    }

    private async Task<List<PotInstanceInfo>> ConvertToPotInstanceInfos(IEnumerable<PotSnapshot> potInstances, DateOnly currentDate, string defaultCurrency)
    {
        List<PotInstanceInfo> potInstanceInfos = [];

        foreach (PotSnapshot potSnapshot in potInstances)
        {
            PotInstanceInfo potInstanceInfo = await Convert(potSnapshot, currentDate, defaultCurrency);
            potInstanceInfos.Add(potInstanceInfo);
        }

        return potInstanceInfos;
    }

    private async Task<PotInstanceInfo> Convert(PotSnapshot potSnapshot, DateOnly currentDate, Currency defaultCurrency)
    {
        PotInstanceInfo potInstanceInfo = new()
        {
            Id = potSnapshot.Pot.Id,
            Name = potSnapshot.Pot.Name,
            IsActive = potSnapshot.Pot.IsActive(currentDate),
            Value = ComputeOriginalValue(potSnapshot)
        };

        potInstanceInfo.NormalizedValue = await currenciesConverter.Convert(potInstanceInfo.Value, defaultCurrency, currentDate);

        return potInstanceInfo;
    }

    private static CurrencyValue ComputeOriginalValue(PotSnapshot potSnapshot)
    {
        if (potSnapshot != null)
        {
            return new CurrencyValue
            {
                Currency = potSnapshot.Pot.Currency,
                Value = potSnapshot.Value,
                Date = potSnapshot.Date
            };
        }

        return null;
    }
}
