using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.ExchangeRateStorage;

internal class ExchangeRatesFile
{
    private readonly string filePath;

    public ExchangeRatesFile(string filePath)
    {
        this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    public bool Exists => File.Exists(filePath);

    public async Task<List<JExchangeRate>> ReadAllAsync(CancellationToken cancellationToken)
    {
        string json = await File.ReadAllTextAsync(filePath, cancellationToken);
        return JsonConvert.DeserializeObject<List<JExchangeRate>>(json);
    }

    public Task SaveAllAsync(IEnumerable<JExchangeRate> conversionRates, CancellationToken cancellationToken)
    {
        IsoDateTimeConverter dateTimeConverter = new()
        {
            DateTimeFormat = "yyyy-MM-dd"
        };
        string json = JsonConvert.SerializeObject(conversionRates, Formatting.Indented, dateTimeConverter);

        return File.WriteAllTextAsync(filePath, json, cancellationToken);
    }
}