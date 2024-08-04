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

namespace DustInTheWind.CurrencyExchange.Application.PresentToday;

public class PresentTodayUseCase : IRequestHandler<PresentTodayRequest, PresentTodayResponse>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ISystemClock systemClock;

    public PresentTodayUseCase(IUnitOfWork unitOfWork, ISystemClock systemClock)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
    }

    public async Task<PresentTodayResponse> Handle(PresentTodayRequest request, CancellationToken cancellationToken)
    {
        PresentTodayResponse response = new();

        List<DateTime> dates;

        if (request.Today)
            dates = new List<DateTime> { systemClock.Today };
        else if (request.Dates?.Count > 0)
            dates = request.Dates;
        else
            dates = new List<DateTime>();

        if (request.CurrencyPair == null)
        {
            if (dates.Count == 0)
            {
                DateTime today = systemClock.Today;

                dates = Enumerable.Range(0, 10)
                    .Select(x => today.AddDays(-x))
                    .ToList();
            }

            List<ExchangeRatesByDateCollection> exchangeRatesByDateCollections = await GetExchangeRatesByDates(dates);

            if (exchangeRatesByDateCollections.Count > 0)
                response.ExchangeRatesByDates = exchangeRatesByDateCollections;
        }
        else
        {
            CurrencyPair currencyPair = request.CurrencyPair;

            if (dates.Count == 0)
            {
                DateTime today = systemClock.Today;

                dates = Enumerable.Range(0, 30)
                    .Select(x => today.AddDays(-x))
                    .ToList();
            }

            response.ExchangeRatesByCurrencies = await GetExchangeRatesByCurrencies(currencyPair, dates);
        }

        return response;
    }

    private async Task<List<ExchangeRatesByDateCollection>> GetExchangeRatesByDates(List<DateTime> dates)
    {
        List<ExchangeRatesByDateCollection> exchangeRatesByDates = new();

        foreach (DateTime date in dates)
        {
            ExchangeRatesByDateCollection exchangeRatesByDateCollection = await GetExchangeRatesByDateCollection(date);
            exchangeRatesByDates.Add(exchangeRatesByDateCollection);
        }

        return exchangeRatesByDates;
    }

    private async Task<ExchangeRatesByDateCollection> GetExchangeRatesByDateCollection(DateTime date)
    {
        return new ExchangeRatesByDateCollection
        {
            Date = date,
            ExchangeRates = (await unitOfWork.ExchangeRateRepository.Get(date))
                .Select(x => new ExchangeRateForCurrency(x))
                .ToList()
        };
    }

    private async Task<List<ExchangeRatesByCurrencyCollection>> GetExchangeRatesByCurrencies(CurrencyPair currencyPair, List<DateTime> dates)
    {
        return new List<ExchangeRatesByCurrencyCollection>
        {
            await GetExchangeRatesByCurrency(currencyPair, dates)
        };
    }

    private async Task<ExchangeRatesByCurrencyCollection> GetExchangeRatesByCurrency(CurrencyPair currencyPair, List<DateTime> dates)
    {
        IEnumerable<ExchangeRate> exchangeRates = await unitOfWork.ExchangeRateRepository.Get(currencyPair, dates);

        return new ExchangeRatesByCurrencyCollection
        {
            CurrencyPair = currencyPair,
            ExchangeRates = exchangeRates
                .Select(x => new ExchangeRateForDate(x))
                .ToList()
        };
    }
}