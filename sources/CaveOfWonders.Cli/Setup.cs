using Autofac;
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
using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;
using Microsoft.Extensions.Configuration;

namespace DustInTheWind.CaveOfWonders.Cli;

internal static class DependenciesSetup
{
    public static void Configure(ContainerBuilder containerBuilder)
    {
        MediatRConfiguration mediatRConfiguration = MediatRConfigurationBuilder.Create("", typeof(PresentPotsRequest).Assembly)
            .WithAllOpenGenericHandlerTypesRegistered()
            .Build();

        containerBuilder.RegisterMediatR(mediatRConfiguration);

        containerBuilder
            .Register(context =>
            {
                IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);

                IConfiguration configuration = configurationBuilder.Build();
                string connectionString = configuration.GetConnectionString("DefaultConnection");

                return new Database(connectionString);
            })
            .AsSelf();

        containerBuilder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();

        containerBuilder.RegisterType<SystemClock>().As<ISystemClock>();
        containerBuilder.RegisterType<BnrService>().As<IBnrService>();
        containerBuilder.RegisterType<InsService>().As<IInsService>();
        containerBuilder.RegisterType<MintosService>().As<IMintosService>();
        containerBuilder.RegisterType<FintownService>().As<IFintownService>();
        containerBuilder.RegisterType<Sheets>().As<ISheets>();
        containerBuilder.RegisterType<Log>().As<ILog>().InstancePerLifetimeScope();
        containerBuilder.RegisterType<FileSystem>().As<IFileSystem>().SingleInstance();

        containerBuilder.RegisterType<CpiImportExportFactory>().As<ICpiImportExportFactory>().SingleInstance();
        containerBuilder.RegisterType<FileCpiImportExport>().AsSelf();
        containerBuilder.RegisterType<WebCpiImportExport>().AsSelf();

        containerBuilder
            .Register(context =>
            {
                ICpiImportExportFactory cpiImportExportFactory = context.Resolve<ICpiImportExportFactory>();
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
            })
            .AsSelf()
            .SingleInstance();
    }
}