using DustInTheWind.CaveOfWonders.Domain;
using System.Runtime.CompilerServices;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.GemStorage;

internal class GemPersister
{
	private readonly string databaseDirectoryPath;

	public GemPersister(string databaseDirectoryPath)
	{
		this.databaseDirectoryPath = databaseDirectoryPath ?? throw new ArgumentNullException(nameof(databaseDirectoryPath));
	}

	public async IAsyncEnumerable<Gem> LoadAsync(List<Pot> pots, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		GemsDirectory gemsDirectory = new(databaseDirectoryPath);

		if (!gemsDirectory.Exists)
			yield break;

		IEnumerable<GemsFile> gemsFiles = gemsDirectory.EnumerateGemsFiles();

		foreach (GemsFile gemsFile in gemsFiles)
		{
			cancellationToken.ThrowIfCancellationRequested();

			IEnumerable<Gem> gems = (await gemsFile.ReadAsync(cancellationToken))
				.Select(x =>
				{
					Gem gem = x.ToGem();
					gem.Pot = pots?.FirstOrDefault(pot => pot.Id == gemsFile.PotId);

					return gem;
				});

			foreach (Gem gem in gems)
			{
				cancellationToken.ThrowIfCancellationRequested();
				yield return gem;
			}
		}
	}

	public async Task SaveAsync(List<Gem> gems, CancellationToken cancellationToken)
	{
		if (gems == null) throw new ArgumentNullException(nameof(gems));

		GemsDirectory gemsDirectory = new(databaseDirectoryPath);

		if (!gemsDirectory.Exists)
			gemsDirectory.Create();

		Dictionary<Guid, List<JGem>> jGemsByPotId = gems
			.Where(x => x.Pot != null)
			.GroupBy(x => x.Pot.Id)
			.ToDictionary(
				x => x.Key,
				x => x
					.Select(gem => gem.ToJGem())
					.ToList());

		DeleteRemovedGemsFiles(gemsDirectory, jGemsByPotId.Keys);

		foreach (KeyValuePair<Guid, List<JGem>> jGemsByPotIdEntry in jGemsByPotId)
		{
			Guid potId = jGemsByPotIdEntry.Key;
			List<JGem> jGems = jGemsByPotIdEntry.Value;

			GemsFile gemsFile = gemsDirectory.GetGemsFile(potId);
			await gemsFile.SaveAsync(jGems, cancellationToken);
		}
	}

	private static void DeleteRemovedGemsFiles(GemsDirectory gemsDirectory, IEnumerable<Guid> potIds)
	{
		HashSet<Guid> potIdsWithGems = potIds.ToHashSet();

		foreach (GemsFile gemsFile in gemsDirectory.EnumerateGemsFiles())
		{
			if (!potIdsWithGems.Contains(gemsFile.PotId))
				gemsFile.Delete();
		}
	}
}