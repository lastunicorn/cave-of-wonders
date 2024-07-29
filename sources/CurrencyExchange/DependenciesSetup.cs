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

using DustInTheWind.CaveOfWonders.Adapters.DataAccess;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;
using DustInTheWind.CaveOfWonders.Adapters.SystemAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.SystemAccess;
using DustInTheWind.CurrencyExchange.Adapters.BnrAccess;
using DustInTheWind.CurrencyExchange.Adapters.InsAccess;
using DustInTheWind.CurrencyExchange.Application.PresentExchangeRate;
using DustInTheWind.CurrencyExchange.Ports.BnrAccess;
using DustInTheWind.CurrencyExchange.Ports.InsAccess;
using Microsoft.Extensions.DependencyInjection;

namespace DustInTheWind.CurrencyExchange;

internal static class DependenciesSetup
{
    public static void Configure(IServiceCollection services)
    {
        services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<PresentExchangeRateRequest>());

        services
            .AddTransient(context => new Database(@"c:\Projects.pet\finanțe\CaveOfWonders\db"));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddTransient<ISystemClock, SystemClock>();
        services.AddTransient<IBnr, Bnr>();
        services.AddTransient<IIns, Ins>();
    }
}