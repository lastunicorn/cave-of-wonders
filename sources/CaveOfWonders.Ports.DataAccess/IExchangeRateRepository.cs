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
using DustInTheWind.CaveOfWonders.Infrastructure;

namespace DustInTheWind.CaveOfWonders.Ports.DataAccess;

public interface IExchangeRateRepository
{
    Task<IEnumerable<ExchangeRate>> Get(CurrencyPair[] currencyPairs);

    Task<IEnumerable<ExchangeRate>> Get(DateTime date);

    Task<IEnumerable<ExchangeRate>> Get(CurrencyPair currencyPair, List<DateTime> dates);

    Task<ExchangeRate> GetForLatestDayAvailable(CurrencyPair currencyPair, DateTime date, bool allowInverted = false);

    Task<IEnumerable<ExchangeRate>> GetForLatestDayAvailable(CurrencyPair[] currencyPairs, DateTime date, bool allowInverted = false);

    Task<IEnumerable<ExchangeRate>> GetByDateInterval(CurrencyPair[] currencyPairs, DateTime? startDate, DateTime? endDate);

    Task<IEnumerable<ExchangeRate>> GetByYear(CurrencyPair[] currencyPairs, uint year, uint? month);

    Task<ImportReport> Import(IEnumerable<ExchangeRate> exchangeRates, CancellationToken cancellationToken);
}