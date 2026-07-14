namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.PotStorage;

internal class PotsDirectory
{
    private const string DirectoryName = "Pots";
    private readonly string directoryPath;

    public PotsDirectory(string rootDirectoryPath)
    {
        if (rootDirectoryPath == null) throw new ArgumentNullException(nameof(rootDirectoryPath));
        directoryPath = Path.Combine(rootDirectoryPath, DirectoryName);
    }

    public bool Exists => Directory.Exists(directoryPath);

    public IEnumerable<PotFile> EnumeratePotFiles()
    {
        return Directory.GetFiles(directoryPath)
            .Select(x => new PotFile(x));
    }

    public PotFile GetPotFile(Guid potId)
    {
        string potFileName = $"{potId:D}.json";
        string potFilePath = Path.Combine(directoryPath, potFileName);

        return new PotFile(potFilePath);
    }

    public void Create()
    {
        Directory.CreateDirectory(directoryPath);
    }
}