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

using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;

public sealed class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly DbContext dbContext;

    private IPotRepository potRepository;
    private IExchangeRateRepository exchangeRateRepository;

    public IPotRepository PotRepository => potRepository ??= new PotRepository(dbContext);

    public IExchangeRateRepository ExchangeRateRepository => exchangeRateRepository ??= new ExchangeRateRepository(dbContext);

    public ICpiRepository CpiRepository => null;

    public IAverageWageRepository AverageWageRepository => null;

    public IGemRepository GemRepository => null;

    public UnitOfWork(DbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        dbContext.SaveChanges();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        dbContext?.Dispose();
    }
}
