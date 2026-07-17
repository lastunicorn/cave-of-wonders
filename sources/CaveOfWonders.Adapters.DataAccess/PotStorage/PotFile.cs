using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.PotStorage;

internal class PotFile
{
    private readonly string filePath;

    public Guid PotId { get; }

    public PotFile(string filePath)
    {
        this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));

        string idAsString = Path.GetFileNameWithoutExtension(filePath);
        PotId = Guid.Parse(idAsString);
    }

    public async Task<JPot> ReadAsync(CancellationToken cancellationToken)
    {
        string json = await File.ReadAllTextAsync(filePath, cancellationToken);
        return JsonConvert.DeserializeObject<JPot>(json);
    }

    public Task SaveAsync(JPot jPot, CancellationToken cancellationToken)
    {
        IsoDateTimeConverter dateTimeConverter = new()
        {
            DateTimeFormat = "yyyy-MM-dd"
        };
        string json = JsonConvert.SerializeObject(jPot, Formatting.Indented, dateTimeConverter);

        return File.WriteAllTextAsync(filePath, json, cancellationToken);
    }
}