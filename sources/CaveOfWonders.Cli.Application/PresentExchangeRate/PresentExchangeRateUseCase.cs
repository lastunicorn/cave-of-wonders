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

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentExchangeRate;

internal class PresentExchangeRateUseCase : IRequestHandler<PresentExchangeRateRequest, PresentExchangeRateResponse>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ISystemClock systemClock;
    private PresentExchangeRateResponse response;

    public PresentExchangeRateUseCase(IUnitOfWork unitOfWork, ISystemClock systemClock)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
    }

    public async Task<PresentExchangeRateResponse> Handle(PresentExchangeRateRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.CurrencyPair))
            throw new Exception("Currency pair value was not provided.");

        CurrencyPair currencyPair = request.CurrencyPair;

        response = new PresentExchangeRateResponse
        {
            CurrencyPair = currencyPair
        };

        if (request.Today)
            await RetrieveForToday(currencyPair);
        else if (request.Date != null)
            await RetrieveByDate(currencyPair, request.Date.Value);
        else if (request.Year != null)
            await RetrieveByYear(currencyPair, request.Year.Value, request.Month);
        else if (request.StartDate != null || request.EndDate != null)
            await RetrieveByDateInterval(currencyPair, request.StartDate, request.EndDate);
        else
            await RetrieveAll(currencyPair);

        return response;
    }

    private Task RetrieveForToday(CurrencyPair currencyPair)
    {
        DateTime dateTime = systemClock.Today;
        return RetrieveByDate(currencyPair, dateTime);
    }

    private async Task RetrieveByDate(CurrencyPair currencyPair, DateTime date)
    {
        ExchangeRate exchangeRate = await unitOfWork.ExchangeRateRepository.GetLatest(currencyPair, date);

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
        response.ExchangeRates = (await unitOfWork.ExchangeRateRepository.GetByYear(currencyPair, year, month))
            .Select(x => new ExchangeRateResponseDto(x))
            .ToList();
    }

    private async Task RetrieveByDateInterval(CurrencyPair currencyPair, DateTime? startDate, DateTime? endDate)
    {
        response.ExchangeRates = (await unitOfWork.ExchangeRateRepository.GetByDateInterval(currencyPair, startDate, endDate))
            .Select(x => new ExchangeRateResponseDto(x))
            .ToList();
    }

    private async Task RetrieveAll(CurrencyPair currencyPair)
    {
        response.ExchangeRates = (await unitOfWork.ExchangeRateRepository.Get(currencyPair))
            .Select(x => new ExchangeRateResponseDto(x))
            .ToList();
    }
}