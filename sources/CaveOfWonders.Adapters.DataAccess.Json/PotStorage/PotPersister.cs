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

			Pot pot = jPot.ToPot();
			pot.Id = potFile.PotId;

			yield return pot;
		}
	}

	public async Task SaveAsync(IEnumerable<Pot> pots, CancellationToken cancellationToken)
	{
		if (pots == null) throw new ArgumentNullException(nameof(pots));

		List<Pot> potList = pots.ToList();

		PotsDirectory potsDirectory = new(databaseDirectoryPath);

		if (!potsDirectory.Exists)
			potsDirectory.Create();
		else
			DeleteRemovedPotFiles(potsDirectory, potList);

		foreach (Pot pot in potList)
		{
			JPot jPot = pot.ToJPot();

			PotFile potFile = potsDirectory.GetPotFile(pot.Id);
			await potFile.SaveAsync(jPot, cancellationToken);
		}
	}

	private static void DeleteRemovedPotFiles(PotsDirectory potsDirectory, List<Pot> pots)
	{
		HashSet<Guid> potIds = pots
			.Select(x => x.Id)
			.ToHashSet();

		foreach (PotFile potFile in potsDirectory.EnumeratePotFiles())
		{
			if (!potIds.Contains(potFile.PotId))
				potFile.Delete();
		}
	}
}