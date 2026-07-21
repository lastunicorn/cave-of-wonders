using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Repositories;
using DustInTheWind.CaveOfWonders.Domain;
using Microsoft.EntityFrameworkCore;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.GemRepositoryTests.TestEnvironments;

internal class SqliteTestBackDoor : SqliteStorageBackDoorBase, ITestBackDoor
{
	public SqliteTestBackDoor(SqliteTempDatabase sqliteTempDatabase)
		: base(sqliteTempDatabase)
	{
	}

	public Task SeedPotAsync(Pot pot, CancellationToken cancellationToken = default)
	{
		PotRepository potRepository = new(DbContext);
		potRepository.Add(pot);

		return Task.CompletedTask;
	}

	public Task SeedGemsAsync(IEnumerable<Gem> gems, CancellationToken cancellationToken = default)
	{
		GemRepository gemRepository = new(DbContext);

		foreach (Gem gem in gems)
			gemRepository.Add(gem);

		return Task.CompletedTask;
	}

	public async Task<List<Gem>> GetAllGemsAsync(CancellationToken cancellationToken = default)
	{
		return await DbContext.Gems
			.Include(x => x.Pot)
			.Include(x => x.Parameters)
			.ToListAsync(cancellationToken);
	}
}