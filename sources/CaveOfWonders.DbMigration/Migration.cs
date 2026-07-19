using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using Microsoft.Extensions.Logging;

namespace DustInTheWind.CaveOfWonders.DbMigration;

/// <summary>
/// Copies every record from a source <see cref="IUnitOfWork"/> into a destination one.
/// Pots are migrated (and saved) before gems, since a gem only carries its pot's id and
/// some adapters enforce that id as a foreign key. Entity kinds not supported by either
/// side (e.g. gems/CPI/average wage on the LiteDb adapter) are skipped rather than failing.
/// </summary>
internal sealed class Migration
{
    private readonly ILogger<Migration> logger;

    public Migration(ILogger<Migration> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task RunAsync(IUnitOfWork source, IUnitOfWork destination, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);

        await MigratePotsAsync(source, destination, cancellationToken);
        await MigrateExchangeRatesAsync(source, destination, cancellationToken);
        await MigrateCpiRecordsAsync(source, destination, cancellationToken);
        await MigrateAverageWagesAsync(source, destination, cancellationToken);
        await MigrateGemsAsync(source, destination, cancellationToken);
    }

    private async Task MigratePotsAsync(IUnitOfWork source, IUnitOfWork destination, CancellationToken cancellationToken)
    {
        int count = 0;

        await foreach (Pot pot in source.PotRepository.GetAllAsync(cancellationToken))
        {
            destination.PotRepository.Add(pot);
            count++;
        }

        await destination.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Migrated {Count} pot{Plural}.", count, count == 1 ? "" : "s");
    }

    private async Task MigrateExchangeRatesAsync(IUnitOfWork source, IUnitOfWork destination, CancellationToken cancellationToken)
    {
        IEnumerable<ExchangeRate> exchangeRates = await source.ExchangeRateRepository.Get(null, cancellationToken);

        int count = 0;

        foreach (ExchangeRate exchangeRate in exchangeRates)
        {
            destination.ExchangeRateRepository.Add(exchangeRate);
            count++;
        }

        await destination.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Migrated {Count} exchange rate{Plural}.", count, count == 1 ? "" : "s");
    }

    private async Task MigrateCpiRecordsAsync(IUnitOfWork source, IUnitOfWork destination, CancellationToken cancellationToken)
    {
        if (source.CpiRepository is null || destination.CpiRepository is null)
        {
            logger.LogInformation("Skipped CPI records (not supported by source or destination adapter).");
            return;
        }

        int count = 0;

        await foreach (Cpi cpi in source.CpiRepository.GetAllAsync(cancellationToken))
        {
            destination.CpiRepository.Add(cpi);
            count++;
        }

        await destination.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Migrated {Count} CPI record{Plural}.", count, count == 1 ? "" : "s");
    }

    private async Task MigrateAverageWagesAsync(IUnitOfWork source, IUnitOfWork destination, CancellationToken cancellationToken)
    {
        if (source.AverageWageRepository is null || destination.AverageWageRepository is null)
        {
            logger.LogInformation("Skipped average wage records (not supported by source or destination adapter).");
            return;
        }

        int count = 0;

        await foreach (AverageWage averageWage in source.AverageWageRepository.GetAllAsync(cancellationToken))
        {
            destination.AverageWageRepository.Add(averageWage);
            count++;
        }

        await destination.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Migrated {Count} average wage record{Plural}.", count, count == 1 ? "" : "s");
    }

    private async Task MigrateGemsAsync(IUnitOfWork source, IUnitOfWork destination, CancellationToken cancellationToken)
    {
        if (source.GemRepository is null || destination.GemRepository is null)
        {
            logger.LogInformation("Skipped gems (not supported by source or destination adapter).");
            return;
        }

        int count = 0;

        await foreach (Gem gem in source.GemRepository.FindAsync(new GemFilter(), cancellationToken))
        {
            destination.GemRepository.Add(gem);
            count++;
        }

        await destination.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Migrated {Count} gem{Plural}.", count, count == 1 ? "" : "s");
    }
}
