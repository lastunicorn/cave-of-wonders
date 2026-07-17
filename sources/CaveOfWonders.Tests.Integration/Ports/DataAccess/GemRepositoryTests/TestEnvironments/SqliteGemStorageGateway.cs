using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Entities;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Repositories;
using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using Microsoft.EntityFrameworkCore;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.GemRepositoryTests.TestEnvironments;

internal class SqliteGemStorageGateway : SqliteStorageGatewayBase, IGemStorageGateway
{
	public SqliteGemStorageGateway(SqliteTempDatabase sqliteTempDatabase)
		: base(sqliteTempDatabase)
	{
	}

	public Task SeedPotAsync(Pot pot, CancellationToken cancellationToken = default)
	{
		PotRepository potRepository = new(DbContext);
		potRepository.Add(pot);

		return Task.CompletedTask;
	}

	public async Task SeedGemsAsync(IEnumerable<Gem> gems, CancellationToken cancellationToken = default)
	{
		await DbContext.Gems.AddRangeAsync(gems
			.Select(x => new GemEntity
			{
				Id = x.Id,
				ExternalId = x.ExternalId,
				Date = x.Date,
				Category = (int)x.Category,
				Amount = x.Amount,
				Description = x.Description,
				PotId = x.Pot.Id,
				Parameters = x.Parameters
					.Select(p => new GemParameterEntity
					{
						Key = p.Key,
						Value = p.Value
					})
					.ToList()
			}), cancellationToken);
	}

	public async Task<List<Gem>> GetAllGemsAsync(CancellationToken cancellationToken = default)
	{
		List<GemEntity> gemEntities = await DbContext.Gems
			.Include(x => x.Pot)
			.Include(x => x.Parameters)
			.ToListAsync(cancellationToken);
		
		return gemEntities
			.Select(MapToDomain)
			.ToList();
	}

	private static Gem MapToDomain(GemEntity entity)
	{
		Gem gem = new()
		{
			Id = entity.Id,
			ExternalId = entity.ExternalId,
			Date = entity.Date,
			Category = (GemCategory)entity.Category,
			Amount = entity.Amount,
			Description = entity.Description,
			Pot = entity.Pot == null
				? null
				: new Pot
				{
					Id = entity.Pot.Id,
					Name = entity.Pot.Name,
					Description = entity.Pot.Description,
					DisplayOrder = entity.Pot.DisplayOrder,
					StartDate = entity.Pot.StartDate,
					EndDate = entity.Pot.EndDate,
					Currency = entity.Pot.Currency
				}
		};

		if (entity.Parameters != null)
		{
			foreach (GemParameterEntity param in entity.Parameters)
				gem.Parameters[param.Key] = param.Value;
		}

		return gem;
	}
}