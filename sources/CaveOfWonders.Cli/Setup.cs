using DustInTheWind.CaveOfWonders.Adapters.BnrAccess;
using DustInTheWind.CaveOfWonders.Adapters.ClockAccess;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;
using DustInTheWind.CaveOfWonders.Adapters.FileAccess;
using DustInTheWind.CaveOfWonders.Adapters.FintownAccess;
using DustInTheWind.CaveOfWonders.Adapters.InsAccess;
using DustInTheWind.CaveOfWonders.Adapters.LogAccess;
using DustInTheWind.CaveOfWonders.Adapters.MintosAccess;
using DustInTheWind.CaveOfWonders.Adapters.SpreadsheetAccess;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentPots;
using DustInTheWind.CaveOfWonders.Ports.BnrAccess;
using DustInTheWind.CaveOfWonders.Ports.ClockAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.FileAccess;
using DustInTheWind.CaveOfWonders.Ports.FintownAccess;
using DustInTheWind.CaveOfWonders.Ports.InsAccess;
using DustInTheWind.CaveOfWonders.Ports.LogAccess;
using DustInTheWind.CaveOfWonders.Ports.MintosAccess;
using DustInTheWind.CaveOfWonders.Ports.SpreadsheetAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace DustInTheWind.CaveOfWonders.Cli;

internal static class DependenciesSetup
{
    public static void Configure(IServiceCollection serviceCollection)
    {
        // Register Configuration
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();
        
        serviceCollection.AddSingleton(configuration);

        // Register MediatR
        serviceCollection.AddMediatR(config =>
            config.RegisterServicesFromAssembly(typeof(PresentPotsRequest).Assembly));

        // Register JSON Database
        serviceCollection.AddScoped(services =>
        {
	        IConfiguration configuration = services.GetRequiredService<IConfiguration>();
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            return new Database(connectionString);
        });

        serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();

        serviceCollection.AddSingleton<ISystemClock, SystemClock>();
        serviceCollection.AddScoped<IBnrService, BnrService>();
        serviceCollection.AddScoped<IInsService, InsService>();
        serviceCollection.AddScoped<IMintosService, MintosService>();
        serviceCollection.AddScoped<IFintownService, FintownService>();
        serviceCollection.AddScoped<ISheets, Sheets>();
        serviceCollection.AddScoped<ILog, Log>();
        serviceCollection.AddSingleton<IFileSystem, FileSystem>();

        serviceCollection.AddSingleton<ICpiImportExportFactory, CpiImportExportFactory>();
        serviceCollection.AddScoped<FileCpiImportExport>();
        serviceCollection.AddScoped<WebCpiImportExport>();

        serviceCollection
            .AddSingleton(context =>
            {
                ICpiImportExportFactory cpiImportExportFactory = context.GetRequiredService<ICpiImportExportFactory>();
                CpiImportExportPool cpiImportExportPool = new(cpiImportExportFactory);
                
                cpiImportExportPool.Add(new CpiImportExportDescriptor
                {
                    Id = new Guid("bb7590ef-6126-4529-8012-b6a8a4c6f903"),
                    Type = typeof(FileCpiImportExport)
                });
                
                cpiImportExportPool.Add(new CpiImportExportDescriptor
                {
                    Id = new Guid("3ff33b30-a149-4f08-b545-e524fd3e4384"),
                    Type = typeof(WebCpiImportExport)
                });
                
                return cpiImportExportPool;
            });
    }
}