using DustInTheWind.CaveOfWonders.DbMigration.DatabaseEndpoints;
using DustInTheWind.CaveOfWonders.DbMigration.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace DustInTheWind.CaveOfWonders.DbMigration;

/// <summary>
/// The composition root: builds the Generic Host (DI, configuration, logging)
/// for one migration run and executes the <see cref="MigrationApplication"/>.
/// </summary>
internal static class MigrationHost
{
    public static async Task<int> RunAsync(MigrationOptions options, CancellationToken cancellationToken)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole(consoleOptions =>
        {
            consoleOptions.FormatterName = PlainConsoleFormatter.FormatterName;
            consoleOptions.LogToStandardErrorThreshold = LogLevel.Error;
        });
        builder.Logging.AddConsoleFormatter<PlainConsoleFormatter, ConsoleFormatterOptions>();

        builder.Services.AddSingleton(options);
        builder.Services.AddSingleton<IDatabaseEndpointFactory, DatabaseEndpointFactory>();
        builder.Services.AddSingleton<Migration>();
        builder.Services.AddSingleton<MigrationApplication>();

        using IHost host = builder.Build();

        MigrationApplication application = host.Services.GetRequiredService<MigrationApplication>();
        return await application.RunAsync(cancellationToken);
    }
}
