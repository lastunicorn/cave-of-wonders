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

using Autofac;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentState;
using DustInTheWind.CaveOfWonders.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;

namespace DustInTheWind.CaveOfWonders.Cli;

internal class DependenciesSetup
{
    public static void Configure(ContainerBuilder containerBuilder)
    {
        MediatRConfiguration mediatRConfiguration = MediatRConfigurationBuilder
            .Create(typeof(PresentStateRequest).Assembly)
            .WithAllOpenGenericHandlerTypesRegistered()
            .Build();

        containerBuilder.RegisterMediatR(mediatRConfiguration);

        containerBuilder.RegisterType<PotRepository>().As<IPotRepository>();
        containerBuilder.RegisterType<ConversionRateRepository>().As<IConversionRateRepository>();
    }
}