using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb.Entities;
using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotRepositoryTests.TestEnvironments;

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
				Snapshots = pot.Snapshots
					.Select(x => new PotSnapshotDbEntity
					{
						Date = x.Date,
						Value = x.Value
					})
					.ToList(),
				Labels = pot.Labels?.Select(x => x.Label).ToList() ?? []
			};

			DbContext.Pots.Insert(potDbEntity);
		}

		return Task.CompletedTask;
	}

	public Task<List<Pot>> GetAllPotsAsync(CancellationToken cancellationToken = default)
	{
		List<Pot> pots = DbContext.Pots
			.FindAll()
			.Select(x => x.ToDomainEntity())
			.ToList();

		return Task.FromResult(pots);
	}
}