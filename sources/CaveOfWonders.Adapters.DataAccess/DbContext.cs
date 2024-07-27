// Currency Exchange
// Copyright (C) 2023 Dust in the Wind
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
using LiteDB;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess;

public sealed class DbContext : IDisposable
{
    private const string DatabaseFilePath = "database.db";
    private readonly LiteDatabase db;

    internal ILiteCollection<ExchangeRateDbEntity> ExchangeRates { get; }

    public DbContext()
    {
        db = new LiteDatabase(DatabaseFilePath);

        ExchangeRates = db.GetCollection<ExchangeRateDbEntity>();
        ExchangeRates.EnsureIndex(x => x.Date);
    }

    public void Dispose()
    {
        db?.Dispose();
    }
}