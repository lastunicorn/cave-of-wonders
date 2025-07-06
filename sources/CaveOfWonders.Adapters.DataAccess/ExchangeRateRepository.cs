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

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;

public class ExchangeRateRepository : IExchangeRateRepository
{
    private readonly Database database;

    public ExchangeRateRepository(Database database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public Task<IEnumerable<ExchangeRate>> Get(CurrencyPair[] currencyPairs)
    {
        IEnumerable<ExchangeRate> exchangeRates = database.ExchangeRates;

        if (currencyPairs != null && currencyPairs.Length > 0)
            exchangeRates = exchangeRates.Where(x => currencyPairs.Contains(x.CurrencyPair));

        return Task.FromResult(exchangeRates);
    }

    public Task<IEnumerable<ExchangeRate>> Get(DateTime date)
    {
        IEnumerable<ExchangeRate> exchangeRates = database.ExchangeRates
            .Where(x => x.Date == date);

        return Task.FromResult(exchangeRates);
    }

    public Task<IEnumerable<ExchangeRate>> Get(CurrencyPair currencyPair, List<DateTime> dates)
    {
        string currencyPairAsString = currencyPair.ToString();
        IEnumerable<ExchangeRate> exchangeRates = database.ExchangeRates
            .Where(x => x.CurrencyPair == currencyPairAsString && dates.Contains(x.Date));

        return Task.FromResult(exchangeRates);
    }

    public Task<ExchangeRate> GetForLatestDayAvailable(CurrencyPair currencyPair, DateTime date, bool allowInverted = false)
    {
        CurrencyPair invertedCurrencyPair = currencyPair.Invert();

        ExchangeRate exchangeRate = database.ExchangeRates
            .Where(x =>
            {
                if (x.Date > date)
                    return false;

                if (x.CurrencyPair == currencyPair)
                    return true;

                if (allowInverted)
                    return x.CurrencyPair == invertedCurrencyPair;

                return false;
            })
            .MaxBy(x => x.Date);

        return Task.FromResult(exchangeRate);
    }

    public Task<IEnumerable<ExchangeRate>> GetForLatestDayAvailable(CurrencyPair[] currencyPairs, DateTime date, bool allowInverted = false)
    {
        IEnumerable<ExchangeRate> exchangeRates = database.ExchangeRates;

        if (currencyPairs != null && currencyPairs.Length > 0)
        {
            CurrencyPair[] currencyPairsAll = ExpandCurrencyPairs(currencyPairs, allowInverted);
            exchangeRates = exchangeRates.Where(x => currencyPairsAll.Contains(x.CurrencyPair));
        }

        exchangeRates = exchangeRates.Where(x => x.Date <= date);

        IGrouping<DateTime, ExchangeRate> exchangeRatesByDate = exchangeRates
            .GroupBy(x => x.Date)
            .OrderByDescending(x => x.Key)
            .FirstOrDefault();

        IEnumerable<ExchangeRate> exchangeRatesFinal = exchangeRatesByDate == null
            ? []
            : exchangeRatesByDate;

        return Task.FromResult(exchangeRatesFinal);
    }

    public Task<IEnumerable<ExchangeRate>> GetByDateInterval(CurrencyPair[] currencyPairs, DateTime? startDate, DateTime? endDate)
    {
        IEnumerable<ExchangeRate> exchangeRates = database.ExchangeRates;

        if (currencyPairs != null && currencyPairs.Length > 0)
            exchangeRates = exchangeRates.Where(x => currencyPairs.Contains(x.CurrencyPair));

        if (startDate != null)
            exchangeRates = exchangeRates.Where(x => x.Date >= startDate.Value);

        if (endDate != null)
            exchangeRates = exchangeRates.Where(x => x.Date <= endDate.Value);

        exchangeRates = exchangeRates.OrderBy(x => x.Date);

        return Task.FromResult(exchangeRates);
    }

    public Task<IEnumerable<ExchangeRate>> GetByYear(CurrencyPair[] currencyPairs, uint year, uint? month)
    {
        IEnumerable<ExchangeRate> exchangeRates = database.ExchangeRates;

        if (currencyPairs != null && currencyPairs.Length > 0)
            exchangeRates = exchangeRates.Where(x => currencyPairs.Contains(x.CurrencyPair));

        exchangeRates = exchangeRates.Where(x => x.Date.Year == year);

        if (month != null)
            exchangeRates = exchangeRates.Where(x => x.Date.Month == month.Value);

        exchangeRates = exchangeRates.OrderBy(x => x.Date);

        return Task.FromResult(exchangeRates);
    }

    public Task<ExchangeRateImportReport> Import(IEnumerable<ExchangeRate> exchangeRates, CancellationToken cancellationToken)
    {
        ImportProcedure importProcedure = new(database.ExchangeRates);
        importProcedure.Execute(exchangeRates);

        return Task.FromResult(importProcedure.Report);
    }

    private static CurrencyPair[] ExpandCurrencyPairs(CurrencyPair[] currencyPairs, bool allowInverted)
    {
        if (currencyPairs == null)
            return [];

        if (allowInverted)
        {
            return currencyPairs
                .SelectMany(x => new[] { x, x.Invert() })
                .Distinct()
                .ToArray();
        }
        else
        {
            return currencyPairs
                .Distinct()
                .ToArray();
        }
    }
}