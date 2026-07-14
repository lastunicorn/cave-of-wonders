using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using System.Runtime.CompilerServices;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.ExchangeRateStorage;

internal class ExchangeRatePersister
{
    private readonly string databaseDirectoryPath;

    public ExchangeRatePersister(string databaseDirectoryPath)
    {
        this.databaseDirectoryPath = databaseDirectoryPath ?? throw new ArgumentNullException(nameof(databaseDirectoryPath));
    }

    public async IAsyncEnumerable<ExchangeRate> LoadAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ExchangeRatesDirectory exchangeRatesDirectory = new(databaseDirectoryPath);

        if (!exchangeRatesDirectory.Exists)
            yield break;

        IEnumerable<ExchangeRatesFile> exchangeRateFiles = exchangeRatesDirectory.EnumerateExchangeRateFiles();

        foreach (ExchangeRatesFile exchangeRatesFile in exchangeRateFiles)
        {
            List<JExchangeRate> jExchangeRates = (await exchangeRatesFile.ReadAllAsync(cancellationToken))
                .OrderByDescending(x => x.Date)
                .ToList();

            IEnumerable<ExchangeRate> exchangeRates = jExchangeRates
                .Select(x => new ExchangeRate
                {
                    CurrencyPair = new CurrencyPair(x.Currency1, x.Currency2),
                    Date = x.Date,
                    Value = x.Value
                });

            foreach (ExchangeRate exchangeRate in exchangeRates)
                yield return exchangeRate;
        }
    }

    public async Task SaveAsync(IEnumerable<ExchangeRate> exchangeRates, CancellationToken cancellationToken)
    {
        ExchangeRatesDirectory exchangeRatesDirectory = new(databaseDirectoryPath);

        if (!exchangeRatesDirectory.Exists)
            exchangeRatesDirectory.Create();

        Dictionary<CurrencyPair, IEnumerable<JExchangeRate>> conversionRateGroups = exchangeRates
            .GroupBy(x => x.CurrencyPair)
            .ToDictionary(x => x.Key, x => x.Select(ToJEntity));

        foreach (KeyValuePair<CurrencyPair, IEnumerable<JExchangeRate>> conversionRateGroup in conversionRateGroups)
        {
            ExchangeRatesFile exchangeRatesFile = exchangeRatesDirectory.GetExchangeRateFile(conversionRateGroup.Key);
            await exchangeRatesFile.SaveAllAsync(conversionRateGroup.Value, cancellationToken);
        }
    }

    private static JExchangeRate ToJEntity(ExchangeRate exchangeRate)
    {
        return new JExchangeRate
        {
            Currency1 = exchangeRate.CurrencyPair.Currency1,
            Currency2 = exchangeRate.CurrencyPair.Currency2,
            Date = exchangeRate.Date,
            Value = exchangeRate.Value
        };
    }
}