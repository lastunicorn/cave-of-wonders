using CaveOfWonders.WebApi.Presentation.Endpoints.Pots;
using CaveOfWonders.WebApi.Presentation.ErrorHandlers;
using DustInTheWind.CaveOfWonders.Adapters.BcrAccess;
using DustInTheWind.CaveOfWonders.Adapters.BnrAccess;
using DustInTheWind.CaveOfWonders.Adapters.ClockAccess;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;
using DustInTheWind.CaveOfWonders.Adapters.FileAccess;
using DustInTheWind.CaveOfWonders.Adapters.FintownAccess;
using DustInTheWind.CaveOfWonders.Adapters.InsAccess;
using DustInTheWind.CaveOfWonders.Adapters.LogAccess;
using DustInTheWind.CaveOfWonders.Adapters.MintosAccess;
using DustInTheWind.CaveOfWonders.Adapters.NonInteractiveUserAccess;
using DustInTheWind.CaveOfWonders.Adapters.PeerBerryAccess;
using DustInTheWind.CaveOfWonders.Adapters.QuanloopAccess;
using DustInTheWind.CaveOfWonders.Adapters.SpreadsheetAccess;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentWealth;
using DustInTheWind.CaveOfWonders.Ports.BcrAccess;
using DustInTheWind.CaveOfWonders.Ports.BnrAccess;
using DustInTheWind.CaveOfWonders.Ports.ClockAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.FileAccess;
using DustInTheWind.CaveOfWonders.Ports.FintownAccess;
using DustInTheWind.CaveOfWonders.Ports.InsAccess;
using DustInTheWind.CaveOfWonders.Ports.LogAccess;
using DustInTheWind.CaveOfWonders.Ports.MintosAccess;
using DustInTheWind.CaveOfWonders.Ports.PeerBerryAccess;
using DustInTheWind.CaveOfWonders.Ports.QuanloopAccess;
using DustInTheWind.CaveOfWonders.Ports.SpreadsheetAccess;
using DustInTheWind.CaveOfWonders.Ports.UserAccess;
using DustInTheWind.ErrorFlow.AspNetCore;
using DustInTheWind.ErrorFlow.AspNetCore.DependencyInjection;
using DustInTheWind.RequestR;
using DustInTheWind.RequestR.Extensions.Microsoft.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Reflection;
using DustInTheWind.OperationEngine;
using DustInTheWind.OperationEngine.Extensions.DependencyInjection;

namespace DustInTheWind.CaveOfWonders.WebApi;

internal static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers().AddApplicationPart(typeof(PotsController).Assembly);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Cave of Wonders API",
                Description = "Web API for the Cave of Wonders financial management system"
            });

            // Set the comments path for the Swagger JSON and UI.
            Assembly assembly = typeof(PotsController).Assembly;
            string xmlFilename = $"{assembly.GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        // Register RequestR
        builder.Services.AddUseCaseEngine(options =>
            options.AddFromAssemblyContaining<PresentWealthRequest>());
		
        // Operation Engine
        builder.Services.AddOperationEngine(config =>
        {
	        config.AddOperationsFromAssemblyContaining<PresentWealthRequest>();
        });

        // Both engines register their entry point as a singleton, which would make every use case
        // and operation be created from the root provider. In a web host they must be created per
        // request, together with the scoped IUnitOfWork they depend on.
        MakeScoped(builder.Services, typeof(RequestBus));
        MakeScoped(builder.Services, typeof(IOperationFactory));
        MakeScoped(builder.Services, typeof(OperationManager));

        // Register application services
        builder.Services.AddSingleton(sp =>
        {
            string connectionString = builder.Configuration.GetConnectionString("Json");
            return new Database(connectionString);
        });
        builder.Services.AddSingleton<IDatabaseConfiguration>(sp =>
        {
            string connectionString = builder.Configuration.GetConnectionString("Json");
            return new DatabaseConfiguration(connectionString);
        });
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddSingleton<ISystemClock, SystemClock>();
        builder.Services.AddSingleton<IBnrService, BnrService>();
        builder.Services.AddSingleton<IInsService, InsService>();
        builder.Services.AddSingleton<IMintosService, MintosService>();
        builder.Services.AddSingleton<IFintownService, FintownService>();
        builder.Services.AddSingleton<IBcrService, BcrService>();
        builder.Services.AddSingleton<IPeerBerryService, PeerBerryService>();
        builder.Services.AddSingleton<IQuanloopService, QuanloopService>();
        builder.Services.AddSingleton<ISheets, Sheets>();
        builder.Services.AddScoped<ILog, Log>();
        builder.Services.AddSingleton<IFileSystem, FileSystem>();

        // The Web API is not interactive, so every confirmation is implicitly accepted.
        builder.Services.AddSingleton<IUserInterface, UserInterface>();

        builder.Services.AddScoped<ICpiImportExportFactory, CpiImportExportFactory>();
        builder.Services.AddScoped<FileCpiImportExport>();
        builder.Services.AddScoped<WebCpiImportExport>();

        builder.Services.AddScoped(context =>
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

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAngularApp", builder =>
            {
                builder.WithOrigins("http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.Services.AddErrorFlow(options =>
        {
            options.AddErrorHandlersFromAssemblyContaining<DefaultErrorHandler>();
            options.AddDefaultErrorHandler<DefaultErrorHandler>();
        });

        WebApplication app = builder.Build();

        app.UseErrorFlow();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors("AllowAngularApp");
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }

    private static void MakeScoped(IServiceCollection services, Type serviceType)
    {
        ServiceDescriptor descriptor = services.Single(x => x.ServiceType == serviceType);
        services.Remove(descriptor);

        ServiceDescriptor scopedDescriptor = descriptor.ImplementationFactory == null
            ? ServiceDescriptor.Scoped(serviceType, descriptor.ImplementationType)
            : ServiceDescriptor.Scoped(serviceType, descriptor.ImplementationFactory);

        services.Add(scopedDescriptor);
    }
}