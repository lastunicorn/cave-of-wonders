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

		PotsDirectory potsDirectory = new(databaseDirectoryPath);

		if (!potsDirectory.Exists)
			potsDirectory.Create();

		foreach (Pot pot in pots)
		{
			JPot jPot = pot.ToJPot();

			PotFile potFile = potsDirectory.GetPotFile(pot.Id);
			await potFile.SaveAsync(jPot, cancellationToken);
		}
	}
}