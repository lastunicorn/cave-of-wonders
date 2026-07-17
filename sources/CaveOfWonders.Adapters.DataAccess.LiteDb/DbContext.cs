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
using LiteDB;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;

public sealed class DbContext : IDisposable
{
    private readonly LiteDatabase db;
    private ILiteCollection<ExchangeRateDbEntity> exchangeRates;
    private ILiteCollection<PotDbEntity> pots;

    internal ExchangeRateTracker ExchangeRateTracker { get; } = new();

    internal ILiteCollection<ExchangeRateDbEntity> ExchangeRates
    {
        get
        {
            if (exchangeRates == null)
            {
                exchangeRates = db.GetCollection<ExchangeRateDbEntity>("exchange_rates");
                exchangeRates.EnsureIndex(static x => x.Date);
            }

            return exchangeRates;
        }
    }

    internal ILiteCollection<PotDbEntity> Pots
    {
        get
        {
            if (pots == null)
            {
                pots = db.GetCollection<PotDbEntity>("pots");
                pots.EnsureIndex(static x => x.Name);
            }

            return pots;
        }
    }

    public DbContext(string filePath)
    {
        // Each DbContext gets its own BsonMapper instead of relying on the default
        // LiteDatabase behavior of falling back to the shared, static BsonMapper.Global.
        // That shared mapper's type-metadata cache isn't safe under concurrent first-time
        // access from multiple LiteDatabase instances (e.g. parallel test runs), which can
        // corrupt reads for types like DateOnly and throw ArgumentOutOfRangeException.
        db = new LiteDatabase(filePath, new BsonMapper());
    }

    public void SaveChanges()
    {
        ExchangeRateTracker.SaveChanges(ExchangeRates);
    }

    public void Dispose()
    {
        db?.Dispose();
    }
}