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
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        response = new PresentExchangeRateResponse();

        /*
         * If "today" or specific date is requested, the exchange rate for today is displayed.
         *  - return requested currency pairs for that day.
         */

        CurrencyPair[] currencyPairs = request.CurrencyPair?.ParseCurrencyPairs().ToArray() ?? [];

        if (request.Today)
            await RetrieveForToday(currencyPairs);
        else if (request.Date != null)
            await RetrieveByDate(currencyPairs, request.Date.Value.Date);
        else if (request.Year != null)
            await RetrieveByYear(currencyPairs, request.Year.Value, request.Month);
        else if (request.StartDate != null || request.EndDate != null)
            await RetrieveByDateInterval(currencyPairs, request.StartDate, request.EndDate);
        else
            await RetrieveAll(currencyPairs);

        return response;
    }

    private Task RetrieveForToday(CurrencyPair[] currencyPairs)
    {
        // If currency pair provided        => 1 currency; multiple dates       => display by currency
        // If currency pair NOT provided    => multiple currencies; multiple dates       => display by currency

        DateTime dateTime = systemClock.Today;
        return RetrieveByDate(currencyPairs, dateTime);
    }

    private async Task RetrieveByDate(CurrencyPair[] currencyPairs, DateTime date)
    {
        IEnumerable<ExchangeRate> exchangeRates = await unitOfWork.ExchangeRateRepository.GetForLatestDayAvailable(currencyPairs, date);

        if (exchangeRates == null)
            throw new ExchangeRateNotFoundException(currencyPairs, date);

        DateTime? actualDate = exchangeRates
            .FirstOrDefault()?.Date;

        DailyExchangeRates dailyExchangeRates = new()
        {
            Date = actualDate ?? date,
            ExchangeRates = exchangeRates
                .Select(x => new ExchangeRateForCurrency
                {
                    CurrencyPair = x.CurrencyPair,
                    Value = x.Value
                })
                .ToList()
        };

        response.DailyExchangeRates = [dailyExchangeRates];

        if (dailyExchangeRates.ExchangeRates.Count > 0 && actualDate != date)
        {
            response.Comments = new ExchangeRatesNotFoundNote
            {
                CurrencyPairs = currencyPairs?.ToList(),
                Date = date
            };
        }
    }

    private async Task RetrieveByYear(CurrencyPair[] currencyPairs, uint year, uint? month)
    {
        response.DailyExchangeRates = (await unitOfWork.ExchangeRateRepository.GetByYear(currencyPairs, year, month))
            .GroupBy(x => x.Date)
            .Select(x => new DailyExchangeRates
            {
                Date = x.Key,
                ExchangeRates = x
                    .Select(y => new ExchangeRateForCurrency
                    {
                        CurrencyPair = y.CurrencyPair,
                        Value = y.Value
                    })
                    .ToList()
            })
            .OrderBy(x => x.Date)
            .ToList();
    }

    private async Task RetrieveByDateInterval(CurrencyPair[] currencyPairs, DateTime? startDate, DateTime? endDate)
    {
        response.DailyExchangeRates = (await unitOfWork.ExchangeRateRepository.GetByDateInterval(currencyPairs, startDate, endDate))
            .GroupBy(x => x.Date)
            .Select(x => new DailyExchangeRates
            {
                Date = x.Key,
                ExchangeRates = x
                    .Select(y => new ExchangeRateForCurrency
                    {
                        CurrencyPair = y.CurrencyPair,
                        Value = y.Value
                    })
                    .ToList()
            })
            .OrderBy(x => x.Date)
            .ToList();
    }

    private async Task RetrieveAll(CurrencyPair[] currencyPairs)
    {
        response.DailyExchangeRates = (await unitOfWork.ExchangeRateRepository.Get(currencyPairs))
            .GroupBy(x => x.Date)
            .Select(x => new DailyExchangeRates
            {
                Date = x.Key,
                ExchangeRates = x
                    .Select(y => new ExchangeRateForCurrency
                    {
                        CurrencyPair = y.CurrencyPair,
                        Value = y.Value
                    })
                    .ToList()
            })
            .OrderBy(x => x.Date)
            .ToList();
    }
}
