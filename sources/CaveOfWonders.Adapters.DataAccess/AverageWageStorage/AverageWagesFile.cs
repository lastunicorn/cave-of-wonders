using Newtonsoft.Json;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.AverageWageStorage;

internal class AverageWagesFile
{
    private readonly string filePath;

    public bool Exists => File.Exists(filePath);

    public AverageWagesFile(string filePath)
    {
        this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    public async Task<IEnumerable<JAverageWageRecord>> ReadAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(filePath))
            return [];

        string json = await File.ReadAllTextAsync(filePath, cancellationToken);

        return string.IsNullOrEmpty(json)
            ? []
            : JsonConvert.DeserializeObject<IEnumerable<JAverageWageRecord>>(json);
    }

    public Task SaveAsync(IEnumerable<JAverageWageRecord> jAverageWageRecords, CancellationToken cancellationToken)
    {
        string json = JsonConvert.SerializeObject(jAverageWageRecords, Formatting.Indented);
        return File.WriteAllTextAsync(filePath, json, cancellationToken);
    }
}