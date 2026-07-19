using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.DbMigration;

/// <summary>
/// Copies every record from a source <see cref="IUnitOfWork"/> into a destination one.
/// Pots are migrated (and saved) before gems, since a gem only carries its pot's id and
/// some adapters enforce that id as a foreign key. Entity kinds not supported by either
/// side (e.g. gems/CPI/average wage on the LiteDb adapter) are skipped rather than failing.
/// </summary>
internal sealed class Migration
{
    private readonly IUnitOfWork source;
    private readonly IUnitOfWork destination;

    public Migration(IUnitOfWork source, IUnitOfWork destination)
    {
        this.source = source ?? throw new ArgumentNullException(nameof(source));
        this.destination = destination ?? throw new ArgumentNullException(nameof(destination));
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        await MigratePotsAsync(cancellationToken);
        await MigrateExchangeRatesAsync(cancellationToken);
        await MigrateCpiRecordsAsync(cancellationToken);
        await MigrateAverageWagesAsync(cancellationToken);
        await MigrateGemsAsync(cancellationToken);
    }

    private async Task MigratePotsAsync(CancellationToken cancellationToken)
    {
        int count = 0;

        await foreach (Pot pot in source.PotRepository.GetAllAsync(cancellationToken))
        {
            destination.PotRepository.Add(pot);
            count++;
        }

        await destination.SaveChangesAsync(cancellationToken);
        Console.WriteLine($"Migrated {count} pot(s).");
    }

    private async Task MigrateExchangeRatesAsync(CancellationToken cancellationToken)
    {
        IEnumerable<ExchangeRate> exchangeRates = await source.ExchangeRateRepository.Get(null, cancellationToken);

        int count = 0;

        foreach (ExchangeRate exchangeRate in exchangeRates)
        {
            destination.ExchangeRateRepository.Add(exchangeRate);
            count++;
        }

        await destination.SaveChangesAsync(cancellationToken);
        Console.WriteLine($"Migrated {count} exchange rate(s).");
    }

    private async Task MigrateCpiRecordsAsync(CancellationToken cancellationToken)
    {
        if (source.CpiRepository is null || destination.CpiRepository is null)
        {
            Console.WriteLine("Skipped CPI records (not supported by source or destination adapter).");
            return;
        }

        int count = 0;

        await foreach (Cpi cpi in source.CpiRepository.GetAllAsync(cancellationToken))
        {
            destination.CpiRepository.Add(cpi);
            count++;
        }

        await destination.SaveChangesAsync(cancellationToken);
        Console.WriteLine($"Migrated {count} CPI record(s).");
    }

    private async Task MigrateAverageWagesAsync(CancellationToken cancellationToken)
    {
        if (source.AverageWageRepository is null || destination.AverageWageRepository is null)
        {
            Console.WriteLine("Skipped average wage records (not supported by source or destination adapter).");
            return;
        }

        int count = 0;

        await foreach (AverageWage averageWage in source.AverageWageRepository.GetAllAsync(cancellationToken))
        {
            destination.AverageWageRepository.Add(averageWage);
            count++;
        }

        await destination.SaveChangesAsync(cancellationToken);
        Console.WriteLine($"Migrated {count} average wage record(s).");
    }

    private async Task MigrateGemsAsync(CancellationToken cancellationToken)
    {
        if (source.GemRepository is null || destination.GemRepository is null)
        {
            Console.WriteLine("Skipped gems (not supported by source or destination adapter).");
            return;
        }

        int count = 0;

        await foreach (Gem gem in source.GemRepository.FindAsync(new GemFilter(), cancellationToken))
        {
            destination.GemRepository.Add(gem);
            count++;
        }

        await destination.SaveChangesAsync(cancellationToken);
        Console.WriteLine($"Migrated {count} gem(s).");
    }
}
