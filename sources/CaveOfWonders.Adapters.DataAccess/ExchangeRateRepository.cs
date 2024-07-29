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

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;

public class ExchangeRateRepository : IExchangeRateRepository
{
    private readonly Database database;

    public ExchangeRateRepository(Database database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public Task<IEnumerable<ExchangeRate>> Get(CurrencyPair currencyPair)
    {
        IEnumerable<ExchangeRate> exchangeRates = database.ExchangeRates
            .Where(x => x.CurrencyPair == currencyPair);

        return Task.FromResult(exchangeRates);
    }

    public Task<IEnumerable<ExchangeRate>> Get(DateTime date)
    {
        IEnumerable<ExchangeRate> exchangeRates = database.ExchangeRates
            .Where(x => x.Date == date);

        return Task.FromResult(exchangeRates);
    }

    public Task<ExchangeRate> GetLatest(CurrencyPair currencyPair, DateTime date)
    {
        string currencyPairAsString = currencyPair.ToString();

        ExchangeRate exchangeRate = database.ExchangeRates
            .Where(x => x.CurrencyPair == currencyPairAsString && x.Date <= date)
            .MaxBy(x => x.Date);

        return Task.FromResult(exchangeRate);
    }

    public Task<IEnumerable<ExchangeRate>> GetByDateInterval(CurrencyPair currencyPair, DateTime? startDate, DateTime? endDate)
    {
        string currencyPairAsString = currencyPair.ToString();

        IEnumerable<ExchangeRate> exchangeRates = database.ExchangeRates
            .Where(x => x.CurrencyPair == currencyPairAsString);

        if (startDate != null)
            exchangeRates = exchangeRates.Where(x => x.Date >= startDate.Value);

        if (endDate != null)
            exchangeRates = exchangeRates.Where(x => x.Date <= endDate.Value);

        exchangeRates = exchangeRates.OrderBy(x => x.Date);

        return Task.FromResult(exchangeRates);
    }

    public Task<IEnumerable<ExchangeRate>> GetByYear(CurrencyPair currencyPair, uint year, uint? month)
    {
        string currencyPairAsString = currencyPair.ToString();

        IEnumerable<ExchangeRate> exchangeRates = database.ExchangeRates
            .Where(x => x.CurrencyPair == currencyPairAsString && x.Date.Year == year);

        if (month != null)
            exchangeRates = exchangeRates.Where(x => x.Date.Month == month.Value);

        exchangeRates = exchangeRates.OrderBy(x => x.Date);

        return Task.FromResult(exchangeRates);
    }

    public Task<ImportReport> Import(IEnumerable<ExchangeRate> exchangeRates)
    {
        ImportProcedure importProcedure = new(database.ExchangeRates);
        importProcedure.Execute(exchangeRates);

        return Task.FromResult(importProcedure.Report);
    }
}