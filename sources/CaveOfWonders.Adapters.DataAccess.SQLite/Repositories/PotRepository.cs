using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Infrastructure;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Repositories;

internal class PotRepository : IPotRepository
{
	private readonly CaveOfWondersDbContext dbContext;

	public PotRepository(CaveOfWondersDbContext dbContext)
	{
		this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
	}

	public IAsyncEnumerable<Pot> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return dbContext.Pots
			.Include(x => x.Snapshots)
			.AsAsyncEnumerable();
	}

	public async Task<Pot> GetAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return await dbContext.Pots
			.Include(x => x.Snapshots)
			.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
	}

	public IAsyncEnumerable<Pot> GetAsync(PotFlexId potFlexId, CancellationToken cancellationToken = default)
	{
		IEnumerable<Pot> pots = dbContext.Pots
			.Include(x => x.Snapshots)
			.AsEnumerable()
			.Where(x => potFlexId.IsMatch(x.Id) || potFlexId.IsMatch(x.Name));

		return pots.ToAsyncEnumerable(cancellationToken);
	}

	public void Add(Pot pot)
	{
		ArgumentNullException.ThrowIfNull(pot);

		dbContext.Pots.Add(pot);
	}

	public void Remove(Pot pot)
	{
		ArgumentNullException.ThrowIfNull(pot);

		Pot entity = dbContext.Pots.Local.FirstOrDefault(x => x.Id == pot.Id);

		if (entity == null)
		{
			entity = new Pot { Id = pot.Id };
			dbContext.Pots.Attach(entity);
		}

		dbContext.Pots.Remove(entity);
	}
}
