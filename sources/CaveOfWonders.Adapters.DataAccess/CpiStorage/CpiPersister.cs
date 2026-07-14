using DustInTheWind.CaveOfWonders.Domain;
using System.Runtime.CompilerServices;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.CpiStorage;

internal class CpiPersister
{
    private readonly string databaseDirectoryPath;

    public CpiPersister(string databaseDirectoryPath)
    {
        this.databaseDirectoryPath = databaseDirectoryPath ?? throw new ArgumentNullException(nameof(databaseDirectoryPath));
    }

    public async IAsyncEnumerable<Cpi> LoadAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (!Directory.Exists(databaseDirectoryPath))
            yield break;

        string filePath = Path.Combine(databaseDirectoryPath, "cpi.json");
        CpiFile cpiFile = new(filePath);

        if (!cpiFile.Exists)
            yield break;

        IEnumerable<JCpi> jCpis = await cpiFile.ReadAsync(cancellationToken);

        IEnumerable<Cpi> cpis = jCpis
            .Select(x => new Cpi
            {
                Year = x.Year,
                Value = x.Value
            });

        foreach (Cpi cpi in cpis)
            yield return cpi;
    }

    public async Task SaveAsync(IEnumerable<Cpi> cpis, CancellationToken cancellationToken)
    {
        if (!Directory.Exists(databaseDirectoryPath))
            Directory.CreateDirectory(databaseDirectoryPath);

        string filePath = Path.Combine(databaseDirectoryPath, "cpi.json");
        CpiFile cpiFile = new(filePath);

        IEnumerable<JCpi> jInflationRecords = cpis
            .Select(x => new JCpi
            {
                Year = x.Year,
                Value = x.Value
            });

        await cpiFile.SaveAsync(jInflationRecords, cancellationToken);
    }
}