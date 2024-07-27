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
using CsvParser.Ports.LogAccess;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CsvParser.Adapters.LogAccess;
using DustInTheWind.CsvParser.Adapters.SheetsAccess;
using DustInTheWind.CsvParser.Application.UseCases.ImportBcr;
using DustInTheWind.CsvParser.Application.UseCases.ImportBrd;
using DustInTheWind.CsvParser.Application.UseCases.ImportBt;
using DustInTheWind.CsvParser.Application.UseCases.ImportIng;
using DustInTheWind.CsvParser.Ports.SheetsAccess;

namespace DustInTheWind.CsvParser;

internal class DependenciesSetup
{
    public static void Configure(ContainerBuilder containerBuilder)
    {
        containerBuilder
            .Register(context => new Database(@"c:\Projects.pet\finanțe\CaveOfWonders\db"))
            .AsSelf();
        containerBuilder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();

        containerBuilder.RegisterType<Log>().As<ILog>();
        containerBuilder.RegisterType<Sheets>().As<ISheets>();

        containerBuilder.RegisterType<ImportBcrGemsUseCase>().AsSelf();
        containerBuilder.RegisterType<ImportIngGemsUseCase>().AsSelf();
        containerBuilder.RegisterType<ImportBrdGemsUseCase>().AsSelf();
        containerBuilder.RegisterType<ImportBtGemsUseCase>().AsSelf();
    }
}