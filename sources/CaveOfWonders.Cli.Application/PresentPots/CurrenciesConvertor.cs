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

using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Infrastructure;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPots;

internal class CurrenciesConvertor
{
    private readonly IUnitOfWork unitOfWork;

    public List<ExchangeRate> UsedExchangeRates { get; } = [];

    public CurrenciesConvertor(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<CurrencyValue> Convert(CurrencyValue originalValue, CurrencyId destinationCurrency, DateTime destinationDate)
    {
        if (originalValue == null)
            return null;

        if (originalValue.Currency != destinationCurrency)
        {
            CurrencyConvertor currencyConverter = await GetConverter(originalValue.Currency, destinationCurrency, destinationDate);

            return new CurrencyValue
            {
                Currency = destinationCurrency,
                Value = currencyConverter.Convert(originalValue.Value),
                Date = currencyConverter.Date
            };
        }

        if (originalValue.Value == 0)
        {
            return new CurrencyValue
            {
                Currency = destinationCurrency,
                Value = 0,
                Date = destinationDate
            };
        }

        return originalValue;
    }

    private async Task<CurrencyConvertor> GetConverter(CurrencyId sourceCurrency, CurrencyId destinationCurrency, DateTime date)
    {
        CurrencyPair currencyPair = new()
        {
            Currency1 = sourceCurrency,
            Currency2 = destinationCurrency
        };

        ExchangeRate exchangeRate = await unitOfWork.ExchangeRateRepository
            .GetForLatestDayAvailable(currencyPair, date, true);

        if (!UsedExchangeRates.Contains(exchangeRate))
            UsedExchangeRates.Add(exchangeRate);

        bool isDirect = sourceCurrency == exchangeRate.CurrencyPair.Currency1;

        return new CurrencyConvertor(exchangeRate, isDirect);
    }
}