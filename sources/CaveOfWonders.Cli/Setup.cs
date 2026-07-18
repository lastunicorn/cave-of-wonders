using DustInTheWind.CaveOfWonders.Adapters.BnrAccess;
using DustInTheWind.CaveOfWonders.Adapters.ClockAccess;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.CompiledModels;
using DustInTheWind.CaveOfWonders.Adapters.FileAccess;
using DustInTheWind.CaveOfWonders.Adapters.FintownAccess;
using DustInTheWind.CaveOfWonders.Adapters.InsAccess;
using DustInTheWind.CaveOfWonders.Adapters.LogAccess;
using DustInTheWind.CaveOfWonders.Adapters.MintosAccess;
using DustInTheWind.CaveOfWonders.Adapters.SpreadsheetAccess;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentPots;
using DustInTheWind.CaveOfWonders.Cli.Utils;
using DustInTheWind.CaveOfWonders.Ports.BnrAccess;
using DustInTheWind.CaveOfWonders.Ports.ClockAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.FileAccess;
using DustInTheWind.CaveOfWonders.Ports.FintownAccess;
using DustInTheWind.CaveOfWonders.Ports.InsAccess;
using DustInTheWind.CaveOfWonders.Ports.LogAccess;
using DustInTheWind.CaveOfWonders.Ports.MintosAccess;
using DustInTheWind.CaveOfWonders.Ports.SpreadsheetAccess;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SQLiteUnitOfWork = DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.UnitOfWork;
using JsonUnitOfWork = DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.UnitOfWork;
using LiteDbUnitOfWork = DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb.UnitOfWork;

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

		// Configure Database
		string databaseType = configuration.GetSection("DatabaseType").Value?.ToLowerInvariant();
		
		switch (databaseType)
		{
			case "sqlite":
				RegisterSqLiteDatabase(serviceCollection);
				break;
			
			case "litedb":
				RegisterLiteDbDatabase(serviceCollection);
				break;

			case "json":
				RegisterJsonDatabase(serviceCollection);
				break;

			default:
				throw new InvalidOperationException("Invalid database type specified in configuration.");
		}
		
		//RegisterSqLiteDatabase(serviceCollection);
		//RegisterJsonDatabase(serviceCollection);

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

	private static void RegisterSqLiteDatabase(IServiceCollection serviceCollection)
	{
		// Register SQLite Database
		serviceCollection.AddDbContext<CaveOfWondersDbContext>((services, options) =>
		{
			IConfiguration configuration = services.GetRequiredService<IConfiguration>();
			string connectionString = new CaveOfWondersConnectionString(configuration.GetConnectionString("SQlite"));

			string dataSource = new SqliteConnectionStringBuilder(connectionString).DataSource;
			string databaseDirectoryPath = Path.GetDirectoryName(Path.GetFullPath(dataSource, AppContext.BaseDirectory));

			if (!string.IsNullOrEmpty(databaseDirectoryPath) && !Directory.Exists(databaseDirectoryPath))
				Directory.CreateDirectory(databaseDirectoryPath);

			options
				.UseSqlite(connectionString)
				.UseModel(CaveOfWondersDbContextModel.Instance);
		});

		serviceCollection.AddScoped<IUnitOfWork>(services =>
		{
			return Measure.Action("Creating UnitOfWork", () =>
			{
				CaveOfWondersDbContext dbContext = Measure.Action("  Resolving DbContext", () => services.GetRequiredService<CaveOfWondersDbContext>());
				Measure.Action("  EnsureCreated", () => dbContext.Database.EnsureCreated());
				
				return new SQLiteUnitOfWork(dbContext);
			});
		});
	}

	private static void RegisterLiteDbDatabase(IServiceCollection serviceCollection)
	{
		// Register LiteDB Database
		serviceCollection.AddScoped(services =>
		{
			return Measure.Action("Creating UnitOfWork", () =>
			{
				IConfiguration configuration = services.GetRequiredService<IConfiguration>();
				string connectionString = new CaveOfWondersConnectionString(configuration.GetConnectionString("LiteDb"));

				return new DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb.DbContext(connectionString);
			});
		});

		serviceCollection.AddScoped<IUnitOfWork, LiteDbUnitOfWork>();
	}

	private static void RegisterJsonDatabase(IServiceCollection serviceCollection)
	{
		// Register JSON Database
		serviceCollection.AddScoped(services =>
		{
			return Measure.Action("Creating UnitOfWork", () =>
			{
				IConfiguration configuration = services.GetRequiredService<IConfiguration>();
				string connectionString = new CaveOfWondersConnectionString(configuration.GetConnectionString("Json"));

				return new Database(connectionString);
			});
		});

		serviceCollection.AddScoped<IUnitOfWork, JsonUnitOfWork>();
	}
}