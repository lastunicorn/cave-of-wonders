namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.GemStorage;

internal class GemsDirectory
{
    private const string DirectoryName = "Gems";
    private readonly string directoryPath;

    public GemsDirectory(string rootDirectoryPath)
    {
        if (rootDirectoryPath == null) throw new ArgumentNullException(nameof(rootDirectoryPath));
        directoryPath = Path.Combine(rootDirectoryPath, DirectoryName);
    }

    public bool Exists => Directory.Exists(directoryPath);

    public IEnumerable<GemsFile> EnumerateGemsFiles()
    {
        return Directory.EnumerateFiles(directoryPath)
            .Select(x => new GemsFile(x));
    }

    public GemsFile GetGemsFile(Guid potId)
    {
        string gemsFileName = $"{potId:D}.json";
        string gemsFilePath = Path.Combine(directoryPath, gemsFileName);

        return new GemsFile(gemsFilePath);
    }

    public void Create()
    {
        Directory.CreateDirectory(directoryPath);
    }
}