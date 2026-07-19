using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb.Entities;
using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.GemRepositoryTests.TestEnvironments;

internal class LiteDbTestBackDoor : LiteDbStorageBackDoorBase, ITestBackDoor
{
	public LiteDbTestBackDoor(LiteDbTempDatabase liteDbTempDatabase)
		: base(liteDbTempDatabase)
	{
	}

	public Task SeedPotAsync(Pot pot, CancellationToken cancellationToken = default)
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
			Labels = pot.Labels?.ToList() ?? []
		};

		DbContext.Pots.Insert(potDbEntity);

		return Task.CompletedTask;
	}

	public Task SeedGemsAsync(IEnumerable<Gem> gems, CancellationToken cancellationToken = default)
	{
		foreach (Gem gem in gems)
		{
			GemDbEntity entity = new()
			{
				Id = gem.Id,
				ExternalId = gem.ExternalId,
				Date = gem.Date,
				Category = (int)gem.Category,
				Amount = gem.Amount,
				Description = gem.Description,
				PotId = gem.Pot.Id,
				Parameters = new Dictionary<string, string>(gem.Parameters)
			};

			DbContext.Gems.Insert(entity);
		}

		return Task.CompletedTask;
	}

	public Task<List<Gem>> GetAllGemsAsync(CancellationToken cancellationToken = default)
	{
		List<Gem> gems = DbContext.Gems
			.FindAll()
			.Select(MapToDomain)
			.ToList();

		return Task.FromResult(gems);
	}

	private Gem MapToDomain(GemDbEntity entity)
	{
		PotDbEntity potEntity = DbContext.Pots.FindById(entity.PotId);

		Pot pot = potEntity == null
			? null
			: new Pot
			{
				Id = potEntity.Id,
				Name = potEntity.Name,
				Description = potEntity.Description,
				DisplayOrder = potEntity.DisplayOrder,
				StartDate = potEntity.StartDate,
				EndDate = potEntity.EndDate,
				Currency = potEntity.Currency
			};

		Gem gem = new()
		{
			Id = entity.Id,
			ExternalId = entity.ExternalId,
			Date = entity.Date,
			Category = (GemCategory)entity.Category,
			Amount = entity.Amount,
			Description = entity.Description,
			Pot = pot
		};

		if (entity.Parameters != null)
		{
			foreach (KeyValuePair<string, string> param in entity.Parameters)
				gem.Parameters[param.Key] = param.Value;
		}

		return gem;
	}
}