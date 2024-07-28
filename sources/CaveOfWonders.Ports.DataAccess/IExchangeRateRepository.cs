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

namespace DustInTheWind.CaveOfWonders.Ports.DataAccess;

public interface IExchangeRateRepository
{
    Task<IEnumerable<ExchangeRate>> Get(CurrencyPair currencyPair);

    Task<IEnumerable<ExchangeRate>> Get(DateTime date);

    Task<ExchangeRate> GetLatest(CurrencyPair currencyPair, DateTime date);

    Task<IEnumerable<ExchangeRate>> GetByDateInterval(CurrencyPair currencyPair, DateTime? startDate, DateTime? endDate);

    Task<IEnumerable<ExchangeRate>> GetByYear(CurrencyPair currencyPair, uint year, uint? month);

    Task<ImportReport> Import(IEnumerable<ExchangeRate> exchangeRates);
}