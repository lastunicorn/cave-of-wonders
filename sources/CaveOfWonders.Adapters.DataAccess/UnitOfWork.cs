// Cave of Wonders
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

using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess;

public class UnitOfWork : IUnitOfWork
{
    private readonly Database database;

    private IPotRepository potRepository;
    private IConversionRateRepository conversionRateRepository;

    public IPotRepository PotRepository => potRepository ??= new PotRepository(database);

    public IConversionRateRepository ConversionRateRepository => conversionRateRepository ??= new ConversionRateRepository(database);

    public UnitOfWork(Database database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database));
        
        database.Load().Wait();
    }

    public Task SaveChanges()
    {
        return database.Save();
    }
}