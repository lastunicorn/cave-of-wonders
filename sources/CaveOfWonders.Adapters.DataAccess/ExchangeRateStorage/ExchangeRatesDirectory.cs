using DustInTheWind.CaveOfWonders.DataTypes;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.ExchangeRateStorage;

internal class ExchangeRatesDirectory
{
    private const string DirectoryName = "exchange-rates";
    private readonly string directoryPath;

    public ExchangeRatesDirectory(string rootDirectoryPath)
    {
        if (rootDirectoryPath == null) throw new ArgumentNullException(nameof(rootDirectoryPath));

        directoryPath = Path.Combine(rootDirectoryPath, DirectoryName);
    }

    public bool Exists => Directory.Exists(directoryPath);

    public IEnumerable<ExchangeRatesFile> EnumerateExchangeRateFiles()
    {
        return Directory.GetFiles(directoryPath)
            .Select(x => new ExchangeRatesFile(x));
    }

    public ExchangeRatesFile GetExchangeRateFile(CurrencyPair currencyPair)
    {
        string fileNameWithoutExtension = currencyPair.ToString().ToLower();
        string fileName = $"{fileNameWithoutExtension}.json";
        string filePath = Path.Combine(directoryPath, fileName);

        return new ExchangeRatesFile(filePath);
    }

    public void Create()
    {
        Directory.CreateDirectory(directoryPath);
    }
}