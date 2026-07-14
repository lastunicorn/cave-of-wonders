using DustInTheWind.CaveOfWonders.Domain;
using System.Runtime.CompilerServices;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.PotStorage;

internal class PotPersister
{
    private readonly string databaseDirectoryPath;

    public PotPersister(string databaseDirectoryPath)
    {
        this.databaseDirectoryPath = databaseDirectoryPath ?? throw new ArgumentNullException(nameof(databaseDirectoryPath));
    }
    
    public async IAsyncEnumerable<Pot> LoadAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        PotsDirectory potsDirectory = new(databaseDirectoryPath);

        if (!potsDirectory.Exists)
            yield break;

        IEnumerable<PotFile> potFiles = potsDirectory.EnumeratePotFiles();

        foreach (PotFile potFile in potFiles)
        {
            JPot jPot = await potFile.ReadAsync(cancellationToken);

            Pot pot = new()
            {
                Id = potFile.PotId,
                Name = jPot.Name,
                Description = jPot.Description,
                DisplayOrder = jPot.DisplayOrder,
                StartDate = jPot.StartDate,
                EndDate = jPot.EndDate,
                Currency = jPot.Currency
            };

            IEnumerable<PotSnapshot> potSnapshots = jPot.Snapshots
                .Select(x => new PotSnapshot
                {
                    Date = x.Date,
                    Value = x.Value,
                    Pot = pot
                });

            if (jPot.Labels != null)
                pot.Labels.AddRange(jPot.Labels);

            pot.Snapshots.AddRange(potSnapshots);

            yield return pot;
        }
    }

    public async Task SaveAsync(IEnumerable<Pot> pots, CancellationToken cancellationToken)
    {
        if (pots == null) throw new ArgumentNullException(nameof(pots));
        
        PotsDirectory potsDirectory = new(databaseDirectoryPath);

        if (!potsDirectory.Exists)
            potsDirectory.Create();

        foreach (Pot pot in pots)
        {
            JPot jPot = new()
            {
                Name = pot.Name,
                Description = pot.Description,
                DisplayOrder = pot.DisplayOrder,
                StartDate = pot.StartDate,
                EndDate = pot.EndDate,
                Currency = pot.Currency,
                Labels = pot.Labels?.ToList(),
                Snapshots = pot.Snapshots
                    .Select(x => new JSnapshot
                    {
                        Date = x.Date,
                        Value = x.Value
                    })
                    .ToList()
            };

            PotFile potFile = potsDirectory.GetPotFile(pot.Id);
            await potFile.SaveAsync(jPot, cancellationToken);
        }
    }
}