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

using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Entities;
using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess;

internal class ExchangeRateDbEntityCache
{
    private readonly DbContext dbContext;

    private readonly HashSet<ExchangeRateDbEntity> existing = new();
    private readonly HashSet<ExchangeRateDbEntity> added = new();
    private readonly HashSet<ExchangeRateDbEntity> deleted = new();

    public ExchangeRateDbEntityCache(DbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public ExchangeRateDbEntity Get(CurrencyPair currencyPair, DateTime date)
    {
        string currencyPairAsString = currencyPair.ToString();

        ExchangeRateDbEntity existingEntity = existing.FirstOrDefault(x => x.Date == date && x.CurrencyPair == currencyPairAsString);
        if (existingEntity != null)
            return existingEntity;

        ExchangeRateDbEntity addedEntity = added.FirstOrDefault(x => x.Date == date && x.CurrencyPair == currencyPairAsString);
        if (addedEntity != null)
            return addedEntity;

        ExchangeRateDbEntity deletedEntity = deleted.FirstOrDefault(x => x.Date == date && x.CurrencyPair == currencyPairAsString);
        if (deletedEntity != null)
            return null;

        ExchangeRateDbEntity dbEntity = dbContext.ExchangeRates.FindOne(x => x.Date == date && x.CurrencyPair == currencyPairAsString);
        if (dbEntity != null)
        {
            existing.Add(dbEntity);
            return dbEntity;
        }

        return null;
    }

    public void Insert(ExchangeRateDbEntity entity)
    {
        if (existing.Contains(entity))
            return;

        if (added.Contains(entity))
            return;

        if (deleted.Contains(entity))
            deleted.Remove(entity);

        added.Add(entity);
    }

    public void Delete(ExchangeRateDbEntity entity)
    {
        if (existing.Contains(entity))
        {
            existing.Remove(entity);
            deleted.Add(entity);
            return;
        }

        if (added.Contains(entity))
        {
            added.Remove(entity);
            return;
        }

        if (deleted.Contains(entity))
            return;

        deleted.Add(entity);
    }
}