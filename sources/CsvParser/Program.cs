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

using Autofac;
using DustInTheWind.CsvParser.Application.Importing;
using DustInTheWind.CsvParser.Application.UseCases.ImportBcr;
using DustInTheWind.CsvParser.Application.UseCases.ImportBrd;
using DustInTheWind.CsvParser.Application.UseCases.ImportBt;
using DustInTheWind.CsvParser.Application.UseCases.ImportCash;
using DustInTheWind.CsvParser.Application.UseCases.ImportGold;
using DustInTheWind.CsvParser.Application.UseCases.ImportIng;
using DustInTheWind.CsvParser.Application.UseCases.ImportRevolut;
using MediatR;

namespace DustInTheWind.CsvParser;

internal class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            IContainer container = ConfigureContainer();

            await ImportBcrSheet(container);
            await ImportIngSheet(container);
            await ImportBrdSheet(container);
            await ImportBtSheet(container);
            await ImportRevolutSheet(container);
            await ImportCashSheet(container);
            await ImportGoldSheet(container);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private static IContainer ConfigureContainer()
    {
        ContainerBuilder containerBuilder = new();
        DependenciesSetup.Configure(containerBuilder);

        return containerBuilder.Build();
    }

    private static async Task ImportBcrSheet(IContainer container)
    {
        await using ILifetimeScope lifetimeScope = container.BeginLifetimeScope();

        IMediator mediator = lifetimeScope.Resolve<IMediator>();

        ImportBcrGemsRequest request = new()
        {
            SourceFilePath = @"c:\Projects.pet\finanțe\CaveOfWonders\import\bcr.csv",
            Overwrite = true
        };

        ImportBcrGemsResponse response = await mediator.Send(request);
        DisplayReports(response.Report);
    }

    private static async Task ImportIngSheet(IContainer container)
    {
        await using ILifetimeScope lifetimeScope = container.BeginLifetimeScope();

        IMediator mediator = lifetimeScope.Resolve<IMediator>();

        ImportIngGemsRequest request = new()
        {
            SourceFilePath = @"c:\Projects.pet\finanțe\CaveOfWonders\import\ing.csv",
            Overwrite = true
        };

        ImportIngGemsResponse response = await mediator.Send(request);
        DisplayReports(response.Report);
    }

    private static async Task ImportBrdSheet(IContainer container)
    {
        await using ILifetimeScope lifetimeScope = container.BeginLifetimeScope();

        IMediator mediator = lifetimeScope.Resolve<IMediator>();

        ImportBrdGemsRequest request = new()
        {
            SourceFilePath = @"c:\Projects.pet\finanțe\CaveOfWonders\import\brd.csv",
            Overwrite = true
        };

        ImportBrdGemsResponse response = await mediator.Send(request);
        DisplayReports(response.Report);
    }

    private static async Task ImportBtSheet(IContainer container)
    {
        await using ILifetimeScope lifetimeScope = container.BeginLifetimeScope();

        IMediator mediator = lifetimeScope.Resolve<IMediator>();

        ImportBtGemsRequest request = new()
        {
            SourceFilePath = @"c:\Projects.pet\finanțe\CaveOfWonders\import\bt.csv",
            Overwrite = true
        };

        ImportBtGemsResponse response = await mediator.Send(request);
        DisplayReports(response.Report);
    }

    private static async Task ImportRevolutSheet(IContainer container)
    {
        await using ILifetimeScope lifetimeScope = container.BeginLifetimeScope();

        IMediator mediator = lifetimeScope.Resolve<IMediator>();

        ImportRevolutGemsRequest request = new()
        {
            SourceFilePath = @"c:\Projects.pet\finanțe\CaveOfWonders\import\revolut.csv",
            Overwrite = true
        };

        ImportRevolutGemsResponse response = await mediator.Send(request);
        DisplayReports(response.Report);
    }

    private static async Task ImportCashSheet(IContainer container)
    {
        await using ILifetimeScope lifetimeScope = container.BeginLifetimeScope();

        IMediator mediator = lifetimeScope.Resolve<IMediator>();

        ImportCashGemsRequest request = new()
        {
            SourceFilePath = @"c:\Projects.pet\finanțe\CaveOfWonders\import\cash.csv",
            Overwrite = true
        };

        ImportCashGemsResponse response = await mediator.Send(request);
        DisplayReports(response.Report);
    }

    private static async Task ImportGoldSheet(IContainer container)
    {
        await using ILifetimeScope lifetimeScope = container.BeginLifetimeScope();

        IMediator mediator = lifetimeScope.Resolve<IMediator>();

        ImportGoldGemsRequest request = new()
        {
            SourceFilePath = @"c:\Projects.pet\finanțe\CaveOfWonders\import\gold.csv",
            Overwrite = true
        };

        ImportGoldGemsResponse response = await mediator.Send(request);
        DisplayReports(response.Report);
    }

    private static void DisplayReports(ImportReport report)
    {
        bool isFirstItem = true;

        foreach (PotImportReport potImportReport in report)
        {
            if (isFirstItem)
                isFirstItem = false;
            else
                Console.WriteLine();

            Console.WriteLine($"Imported gems into {potImportReport.Pot.Name} ({potImportReport.Pot.Id:D})");
            Console.WriteLine($"  - Added: {potImportReport.AddCount}");
            Console.WriteLine($"  - Skipped: {potImportReport.SkipCount}");
        }
    }
}