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

    private readonly List<ExchangeRate> usedExchangeRates = new();

    public PresentStateUseCase(ISystemClock systemClock, IUnitOfWork unitOfWork)
    {
        this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<PresentStateResponse> Handle(PresentStateRequest request, CancellationToken cancellationToken)
    {
        DateTime currentDate = request.Date ?? systemClock.Today;
        string defaultCurrency = request.Currency ?? "RON";

        IEnumerable<PotInstance> potInstances = await RetrievePotInstances(currentDate, request.IncludeInactive);
        List<PotInstanceInfo> potInstanceInfos = await ConvertToPotInstanceInfos(potInstances, currentDate, defaultCurrency);

        return new PresentStateResponse
        {
            Date = currentDate,
            PotInstances = potInstanceInfos,
            ConversionRates = usedExchangeRates
                .Select(x => new ExchangeRateInfo(x))
                .ToList(),
            Total = new CurrencyValue
            {
                Value = potInstanceInfos.Sum(x => x.NormalizedValue?.Value ?? 0),
                Currency = defaultCurrency
            }
        };
    }

    private async Task<IEnumerable<PotInstance>> RetrievePotInstances(DateTime date, bool includeInactive)
    {
        IEnumerable<PotInstance> potInstances = await unitOfWork.PotRepository.GetInstances(date, DateMatchingMode.LastAvailable, includeInactive);
        potInstances = potInstances.OrderBy(x => x.Pot.DisplayOrder);

        return potInstances;
    }

    private async Task<List<PotInstanceInfo>> ConvertToPotInstanceInfos(IEnumerable<PotInstance> potInstances, DateTime currentDate, string defaultCurrency)
    {
        List<PotInstanceInfo> potInstanceInfos = new();

        foreach (PotInstance potInstance in potInstances)
        {
            PotInstanceInfo potInstanceInfo = await Convert(potInstance, currentDate, defaultCurrency);
            potInstanceInfos.Add(potInstanceInfo);
        }

        return potInstanceInfos;
    }

    private async Task<PotInstanceInfo> Convert(PotInstance potInstance, DateTime currentDate, CurrencyId defaultCurrency)
    {
        PotInstanceInfo potInstanceInfo = new()
        {
            Id = potInstance.Pot.Id,
            Name = potInstance.Pot.Name,
            IsActive = potInstance.Pot.IsActive(currentDate),
            OriginalValue = ComputeOriginalValue(potInstance)
        };

        potInstanceInfo.NormalizedValue = await ComputeNormalizedValue(currentDate, defaultCurrency, potInstanceInfo.OriginalValue);

        return potInstanceInfo;
    }

    private static CurrencyValue ComputeOriginalValue(PotInstance potInstance)
    {
        if (potInstance.Gem != null)
        {
            return new CurrencyValue
            {
                Currency = potInstance.Pot.Currency,
                Value = potInstance.Gem.Value,
                Date = potInstance.Gem.Date
            };
        }

        return null;
    }

    private async Task<CurrencyValue> ComputeNormalizedValue(DateTime currentDate, CurrencyId defaultCurrency, CurrencyValue originalValue)
    {
        if (originalValue.Currency != defaultCurrency)
        {
            CurrencyPair currencyPair = new()
            {
                Currency1 = originalValue.Currency,
                Currency2 = defaultCurrency
            };

            ExchangeRate exchangeRate = await unitOfWork.ExchangeRateRepository.GetLatest(currencyPair, currentDate, true);
            return TryConvert(originalValue, defaultCurrency, exchangeRate);
        }

        if (originalValue.Value == 0)
        {
            return new CurrencyValue
            {
                Currency = defaultCurrency,
                Value = 0,
                Date = originalValue.Date
            };
        }

        return originalValue;
    }

    private CurrencyValue TryConvert(CurrencyValue originalValue, CurrencyId destinationCurrency, ExchangeRate exchangeRate)
    {
        if (exchangeRate == null)
            return null;

        if (!usedExchangeRates.Contains(exchangeRate))
            usedExchangeRates.Add(exchangeRate);

        return new CurrencyValue
        {
            Currency = destinationCurrency,
            Value = originalValue.Currency == exchangeRate.CurrencyPair.Currency1
                ? exchangeRate.Convert(originalValue.Value)
                : exchangeRate.ConvertBack(originalValue.Value),
            Date = exchangeRate.Date
        };
    }
}