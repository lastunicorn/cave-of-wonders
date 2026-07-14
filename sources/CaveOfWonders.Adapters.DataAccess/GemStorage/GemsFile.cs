using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.GemStorage;

internal class GemsFile
{
    private readonly string filePath;

    public Guid PotId { get; }

    public GemsFile(string filePath)
    {
        this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));

        string idAsString = Path.GetFileNameWithoutExtension(filePath);
        PotId = Guid.Parse(idAsString);
    }

    public async Task<List<JGem>> ReadAsync(CancellationToken cancellationToken)
    {
        string json = await File.ReadAllTextAsync(filePath, cancellationToken);
        return JsonConvert.DeserializeObject<List<JGem>>(json);
    }

    public Task SaveAsync(IEnumerable<JGem> jGems, CancellationToken cancellationToken)
    {
        IsoDateTimeConverter dateTimeConverter = new()
        {
            DateTimeFormat = "yyy-MM-dd HH:mm:ss"
        };
        string json = JsonConvert.SerializeObject(jGems, Formatting.Indented, dateTimeConverter);

        return File.WriteAllTextAsync(filePath, json, cancellationToken);
    }
}