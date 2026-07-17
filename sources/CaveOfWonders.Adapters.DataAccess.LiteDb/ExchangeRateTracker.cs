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

using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb.Entities;
using DustInTheWind.CaveOfWonders.Domain;
using LiteDB;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;

internal sealed class ExchangeRateTracker
{
    private readonly Dictionary<int, (ExchangeRate Domain, ExchangeRateDbEntity DbEntity)> tracked = new();
    private readonly List<(ExchangeRate Domain, ExchangeRateDbEntity DbEntity)> pendingInserts = new();

    public ExchangeRate GetOrAttach(ExchangeRateDbEntity dbEntity)
    {
        if (tracked.TryGetValue(dbEntity.Id, out (ExchangeRate Domain, ExchangeRateDbEntity DbEntity) entry))
            return entry.Domain;

        ExchangeRate domain = new()
        {
            Date = dbEntity.Date,
            CurrencyPair = dbEntity.CurrencyPair,
            Value = dbEntity.Value
        };

        tracked[dbEntity.Id] = (domain, dbEntity);
        return domain;
    }

    public void TrackNew(ExchangeRate exchangeRate)
    {
        pendingInserts.Add((exchangeRate, new ExchangeRateDbEntity(exchangeRate)));
    }

    public void SaveChanges(ILiteCollection<ExchangeRateDbEntity> collection)
    {
        foreach ((ExchangeRate domain, ExchangeRateDbEntity dbEntity) in pendingInserts)
        {
            collection.Insert(dbEntity);
            tracked[dbEntity.Id] = (domain, dbEntity);
        }

        pendingInserts.Clear();

        foreach ((ExchangeRate domain, ExchangeRateDbEntity dbEntity) in tracked.Values)
        {
            dbEntity.Value = domain.Value;
            collection.Update(dbEntity);
        }
    }
}
