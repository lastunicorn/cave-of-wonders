﻿// Cave of Wonders
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

using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb.Entities;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Infrastructure;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using LiteDB;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;

public class ExchangeRateRepository : IExchangeRateRepository
{
    private readonly DbContext dbContext;

    public ExchangeRateRepository(DbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public Task<IEnumerable<ExchangeRate>> Get(CurrencyPair[] currencyPairs)
    {
        ILiteQueryable<ExchangeRateDbEntity> query = dbContext.ExchangeRates.Query();

        if (currencyPairs != null && currencyPairs.Length > 0)
            query = query.Where(x => currencyPairs.Contains(x.CurrencyPair));

        IEnumerable<ExchangeRate> exchangeRates = query
             .OrderBy(x => x.Date)
             .ToEnumerable()
             .Select(x => new ExchangeRate
             {
                 Date = x.Date,
                 CurrencyPair = x.CurrencyPair,
                 Value = x.Value
             });

        return Task.FromResult(exchangeRates);
    }

    public Task<IEnumerable<ExchangeRate>> Get(DateTime date)
    {
        IEnumerable<ExchangeRate> exchangeRates = dbContext.ExchangeRates.Query()
            .Where(x => x.Date == date)
            .ToEnumerable()
            .Select(x => new ExchangeRate
            {
                Date = x.Date,
                CurrencyPair = x.CurrencyPair,
                Value = x.Value
            });

        return Task.FromResult(exchangeRates);
    }

    public Task<IEnumerable<ExchangeRate>> Get(CurrencyPair currencyPair, List<DateTime> dates)
    {
        throw new NotImplementedException();
    }

    public Task<ExchangeRate> GetForLatestDayAvailable(CurrencyPair currencyPair, DateTime date, bool allowInverted = false)
    {
        string currencyPairAsString = currencyPair.ToString();

        ExchangeRateDbEntity exchangeRateDbEntity = dbContext.ExchangeRates.Query()
            .Where(x => x.CurrencyPair == currencyPairAsString && x.Date <= date)
            .OrderByDescending(x => x.Date)
            .FirstOrDefault();

        ExchangeRate exchangeRate = new()
        {
            Date = exchangeRateDbEntity.Date,
            CurrencyPair = exchangeRateDbEntity.CurrencyPair,
            Value = exchangeRateDbEntity.Value
        };

        return Task.FromResult(exchangeRate);
    }

    public Task<IEnumerable<ExchangeRate>> GetForLatestDayAvailable(CurrencyPair[] currencyPairs, DateTime date, bool allowInverted = false)
    {
        string[] currencyPairsAsStrings = currencyPairs
            .Select(x => x.ToString())
            .ToArray();

        IEnumerable<ExchangeRate> exchangeRates = dbContext.ExchangeRates.Query()
            .Where(x => currencyPairsAsStrings.Contains(x.CurrencyPair) && x.Date <= date)
            .ToList()
            .GroupBy(x => x.Date)
            .OrderByDescending(x => x.Key)
            .FirstOrDefault()
            .Select(x => new ExchangeRate
            {
                Date = x.Date,
                CurrencyPair = x.CurrencyPair,
                Value = x.Value
            });

        return Task.FromResult(exchangeRates);
    }

    public Task<IEnumerable<ExchangeRate>> GetByDateInterval(CurrencyPair[] currencyPairs, DateTime? startDate, DateTime? endDate)
    {
        ILiteQueryable<ExchangeRateDbEntity> query = dbContext.ExchangeRates.Query();

        if (currencyPairs != null && currencyPairs.Length > 0)
            query = query.Where(x => currencyPairs.Contains(x.CurrencyPair));

        if (startDate != null)
            query = query.Where(x => x.Date >= startDate.Value);

        if (endDate != null)
            query = query.Where(x => x.Date <= endDate.Value);

        query = query.OrderBy(x => x.Date);

        IEnumerable<ExchangeRate> exchangeRates = query
            .ToEnumerable()
            .Select(x => new ExchangeRate
            {
                Date = x.Date,
                CurrencyPair = x.CurrencyPair,
                Value = x.Value
            });

        return Task.FromResult(exchangeRates);
    }

    public Task<IEnumerable<ExchangeRate>> GetByYear(CurrencyPair currencyPair, uint year, uint? month)
    {
        string currencyPairAsString = currencyPair.ToString();

        ILiteQueryable<ExchangeRateDbEntity> query = dbContext.ExchangeRates.Query()
            .Where(x => x.CurrencyPair == currencyPairAsString && x.Date.Year == year);

        if (month != null)
            query = query.Where(x => x.Date.Month == month.Value);

        query = query.OrderBy(x => x.Date);

        IEnumerable<ExchangeRate> exchangeRates = query
            .ToEnumerable()
            .Select(x => new ExchangeRate
            {
                Date = x.Date,
                CurrencyPair = x.CurrencyPair,
                Value = x.Value
            });

        return Task.FromResult(exchangeRates);
    }

    public Task<IEnumerable<ExchangeRate>> GetByYear(CurrencyPair[] currencyPairs, uint year, uint? month)
    {
        throw new NotImplementedException();
    }

    public Task<ExchangeRateImportReport> Import(IEnumerable<ExchangeRate> exchangeRates, CancellationToken cancellationToken)
    {
        ImportProcedure importProcedure = new(dbContext.ExchangeRates);
        importProcedure.Execute(exchangeRates);

        return Task.FromResult(importProcedure.Report);
    }
}