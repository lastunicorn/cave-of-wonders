using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb.Entities;
using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotSnapshotRepositoryTests.TestEnvironments;

internal class LiteDbTestBackDoor : LiteDbStorageBackDoorBase, ITestBackDoor
{
	public LiteDbTestBackDoor(LiteDbTempDatabase liteDbTempDatabase)
		: base(liteDbTempDatabase)
	{
	}

	public Task SeedPotsAsync(IEnumerable<Pot> pots, CancellationToken cancellationToken = default)
	{
		foreach (Pot pot in pots)
		{
			PotDbEntity potDbEntity = new()
			{
				Id = pot.Id,
				Name = pot.Name,
				Description = pot.Description,
				DisplayOrder = pot.DisplayOrder,
				StartDate = pot.StartDate,
				EndDate = pot.EndDate,
				Currency = pot.Currency,
				Snapshots = [],
				Labels = pot.Labels?.Select(x => x.Label).ToList() ?? []
			};

			DbContext.Pots.Insert(potDbEntity);
		}

		return Task.CompletedTask;
	}

	public Task SeedPotSnapshotsAsync(IEnumerable<PotSnapshot> potSnapshots, CancellationToken cancellationToken = default)
	{
		foreach (IGrouping<Guid, PotSnapshot> group in potSnapshots.GroupBy(x => x.Pot.Id))
		{
			PotDbEntity potDbEntity = DbContext.Pots.FindById(group.Key);

			if (potDbEntity == null)
				continue;

			potDbEntity.Snapshots.AddRange(group.Select(x => new PotSnapshotDbEntity
			{
				Date = x.Date,
				Value = x.Value
			}));

			DbContext.Pots.Update(potDbEntity);
		}

		return Task.CompletedTask;
	}

	public Task<List<PotSnapshot>> GetSnapshotsByPotIdAsync(Guid potId, CancellationToken cancellationToken = default)
	{
		PotDbEntity potDbEntity = DbContext.Pots.FindById(potId);
		List<PotSnapshot> snapshots = potDbEntity?.ToPotSnapshots(potDbEntity.ToDomainEntity()) ?? [];

		return Task.FromResult(snapshots);
	}
}
