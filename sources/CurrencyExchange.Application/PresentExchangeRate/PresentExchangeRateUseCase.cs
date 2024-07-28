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

namespace DustInTheWind.CurrencyExchange.Application.PresentExchangeRate;

internal class PresentExchangeRateUseCase : IRequestHandler<PresentExchangeRateRequest, PresentExchangeRateResponse>
{
    private readonly IExchangeRateRepository exchangeRateRepository;
    private readonly ISystemClock systemClock;
    private PresentExchangeRateResponse response;

    public PresentExchangeRateUseCase(IExchangeRateRepository exchangeRateRepository, ISystemClock systemClock)
    {
        this.exchangeRateRepository = exchangeRateRepository ?? throw new ArgumentNullException(nameof(exchangeRateRepository));
        this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
    }

    public async Task<PresentExchangeRateResponse> Handle(PresentExchangeRateRequest request, CancellationToken cancellationToken)
    {
        if (request.CurrencyPair.IsEmpty)
            throw new Exception("Currency pair value was not provided.");

        response = new PresentExchangeRateResponse
        {
            CurrencyPair = request.CurrencyPair
        };

        if (request.Today)
            await RetrieveForToday(request.CurrencyPair);
        else if (request.Date != null)
            await RetrieveByDate(request.CurrencyPair, request.Date.Value);
        else if (request.Year != null)
            await RetrieveByYear(request.CurrencyPair, request.Year.Value, request.Month);
        else if (request.StartDate != null || request.EndDate != null)
            await RetrieveByDateInterval(request.CurrencyPair, request.StartDate, request.EndDate);
        else
            await RetrieveAll(request.CurrencyPair);

        return response;
    }

    private Task RetrieveForToday(CurrencyPair currencyPair)
    {
        DateTime dateTime = systemClock.Today;
        return RetrieveByDate(currencyPair, dateTime);
    }

    private async Task RetrieveByDate(CurrencyPair currencyPair, DateTime date)
    {
        ExchangeRate exchangeRate = await exchangeRateRepository.GetLatest(currencyPair, date);

        if (exchangeRate == null)
            throw new Exception($"Exchange rate for {currencyPair} and date {date:d} was not found.");

        response.ExchangeRates = new List<ExchangeRateResponseDto>
        {
            new(exchangeRate)
        };

        if (exchangeRate.Date != date)
            response.Comments = $"Exchange rate for {currencyPair} was not found for the specified date {date:d}. The last available value was returned.";
    }

    private async Task RetrieveByYear(CurrencyPair currencyPair, uint year, uint? month)
    {
        response.ExchangeRates = (await exchangeRateRepository.GetByYear(currencyPair, year, month))
            .Select(x => new ExchangeRateResponseDto(x))
            .ToList();
    }

    private async Task RetrieveByDateInterval(CurrencyPair currencyPair, DateTime? startDate, DateTime? endDate)
    {
        response.ExchangeRates = (await exchangeRateRepository.GetByDateInterval(currencyPair, startDate, endDate))
            .Select(x => new ExchangeRateResponseDto(x))
            .ToList();
    }

    private async Task RetrieveAll(CurrencyPair currencyPair)
    {
        response.ExchangeRates = (await exchangeRateRepository.Get(currencyPair))
            .Select(x => new ExchangeRateResponseDto(x))
            .ToList();
    }
}