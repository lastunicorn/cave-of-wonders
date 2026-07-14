using Newtonsoft.Json;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.CpiStorage;

internal class CpiFile
{
    private readonly string filePath;

    public bool Exists => File.Exists(filePath);

    public CpiFile(string filePath)
    {
        this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    public async Task<IEnumerable<JCpi>> ReadAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(filePath))
            return [];

        string json = await File.ReadAllTextAsync(filePath, cancellationToken);

        return string.IsNullOrEmpty(json)
            ? []
            : JsonConvert.DeserializeObject<IEnumerable<JCpi>>(json);
    }

    public Task SaveAsync(IEnumerable<JCpi> jCpi, CancellationToken cancellationToken)
    {
        string json = JsonConvert.SerializeObject(jCpi, Formatting.Indented);
        return File.WriteAllTextAsync(filePath, json, cancellationToken);
    }
}