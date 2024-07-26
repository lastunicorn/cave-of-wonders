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
using DustInTheWind.CsvParser.Application.Importing;
using DustInTheWind.CsvParser.Application.UseCases.ImportBcr;
using DustInTheWind.CsvParser.Application.UseCases.ImportIng;

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

        ImportBcrGemsUseCase useCase = lifetimeScope.Resolve<ImportBcrGemsUseCase>();

        useCase.SourceFilePath = @"c:\Users\alexandruiuga\OneDrive - Nagarro\Desktop\bcr.csv";
        useCase.Overwrite = true;

        ImportBcrResponse response = await useCase.Execute();
        DisplayReports(response.Report);
    }

    private static async Task ImportIngSheet(IContainer container)
    {
        await using ILifetimeScope lifetimeScope = container.BeginLifetimeScope();

        ImportIngGemsUseCase useCase = lifetimeScope.Resolve<ImportIngGemsUseCase>();

        useCase.SourceFilePath = @"c:\Users\alexandruiuga\OneDrive - Nagarro\Desktop\ing.csv";
        useCase.Overwrite = true;

        ImportIngResponse response = await useCase.Execute();
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