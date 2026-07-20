using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb.Entities;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb.Utils;
using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;

public class GemRepository : IGemRepository
{
	private readonly DbContext dbContext;

	public GemRepository(DbContext dbContext)
	{
		this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
	}

	public IAsyncEnumerable<Gem> GetByPotIdAsync(Guid potId, CancellationToken cancellationToken = default)
	{
		IEnumerable<Gem> gems = dbContext.Gems
			.FindAll()
			.Where(x => x.PotId == potId)
			.Select(MapToDomain);

		return gems.ToAsyncEnumerable(cancellationToken);
	}

	public Task<Gem> GetByExternalIdAsync(Guid potId, string gemExternalId, CancellationToken cancellationToken = default)
	{
		GemDbEntity entity = dbContext.Gems
			.FindAll()
			.FirstOrDefault(x => x.PotId == potId && x.ExternalId == gemExternalId);

		Gem gem = entity == null ? null : MapToDomain(entity);

		return Task.FromResult(gem);
	}

	public IAsyncEnumerable<Gem> FindAsync(GemFilter filter, CancellationToken cancellationToken = default)
	{
		IEnumerable<GemDbEntity> entities = dbContext.Gems.FindAll();

		if (filter.PotId != null)
			entities = entities.Where(x => x.PotId == filter.PotId.Value);

		if (filter.Date != null)
			entities = entities.Where(x => DateOnly.FromDateTime(x.Date) == filter.Date.Value);

		if (filter.Month != null)
			entities = entities.Where(x => x.Date.Year == filter.Month.Value.Year && x.Date.Month == filter.Month.Value.Month);

		if (filter.IncludeCategories?.Count > 0)
			entities = entities.Where(x => filter.IncludeCategories.Contains((GemCategory)x.Category));

		if (filter.ExcludeCategories?.Count > 0)
			entities = entities.Where(x => !filter.ExcludeCategories.Contains((GemCategory)x.Category));

		if (filter.ExternalId != null)
			entities = entities.Where(x => x.ExternalId == filter.ExternalId);

		IEnumerable<Gem> gems = entities.Select(MapToDomain);

		return gems.ToAsyncEnumerable(cancellationToken);
	}

	public IAsyncEnumerable<Gem> FindByDateAsync(Guid potId, DateOnly date, CancellationToken cancellationToken = default)
	{
		IEnumerable<Gem> gems = dbContext.Gems
			.FindAll()
			.Where(x => x.PotId == potId && DateOnly.FromDateTime(x.Date) == date)
			.Select(MapToDomain);

		return gems.ToAsyncEnumerable(cancellationToken);
	}

	public IAsyncEnumerable<Gem> FindByMonthAsync(Guid potId, DustInTheWind.CaveOfWonders.Infrastructure.MonthDate month, CancellationToken cancellationToken = default)
	{
		IEnumerable<Gem> gems = dbContext.Gems
			.FindAll()
			.Where(x => x.PotId == potId && x.Date.Year == month.Year && x.Date.Month == month.Month)
			.Select(MapToDomain);

		return gems.ToAsyncEnumerable(cancellationToken);
	}

	public void Add(Gem gem)
	{
		ArgumentNullException.ThrowIfNull(gem);

		GemDbEntity entity = new()
		{
			Id = gem.Id,
			ExternalId = gem.ExternalId,
			Date = gem.Date,
			Category = (int)gem.Category,
			Amount = gem.Amount,
			Description = gem.Description,
			PotId = gem.Pot.Id,
			Parameters = gem.Parameters.ToDictionary(x => x.Key, x => x.Value)
		};

		dbContext.Gems.Insert(entity);
	}

	public void Remove(Gem gem)
	{
		ArgumentNullException.ThrowIfNull(gem);

		dbContext.Gems.Delete(gem.Id);
	}

	private Gem MapToDomain(GemDbEntity entity)
	{
		PotDbEntity potEntity = dbContext.Pots.FindById(entity.PotId);

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
				gem.Parameters.Add(new GemParameter
				{
					Key = param.Key,
					Value = param.Value
				});
		}

		return gem;
	}
}