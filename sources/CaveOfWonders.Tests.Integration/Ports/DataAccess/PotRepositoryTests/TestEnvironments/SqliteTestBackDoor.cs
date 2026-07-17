using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Entities;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Repositories;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotRepositoryTests.TestEnvironments;

internal class SqliteTestBackDoor : SqliteStorageBackDoorBase, ITestBackDoor
{
	public SqliteTestBackDoor(SqliteTempDatabase sqliteTempDatabase)
		: base(sqliteTempDatabase)
	{
	}

	public async Task SeedPotsAsync(IEnumerable<Pot> pots, CancellationToken cancellationToken = default)
	{
		await DbContext.Pots.AddRangeAsync(pots
			.Select(x => new PotEntity
			{
				Id = x.Id,
				Name = x.Name,
				Description = x.Description,
				DisplayOrder = x.DisplayOrder,
				StartDate = x.StartDate,
				EndDate = x.EndDate,
				Currency = x.Currency,
				Snapshots = x.Snapshots
					.Select(z => new PotSnapshotEntity
					{
						Date = z.Date,
						Value = z.Value
					})
					.ToList(),
				Labels = x.Labels
					.Select(z => new PotLabelEntity
					{
						Label = z
					})
					.ToList()
			}), cancellationToken);
	}

	public Task<List<Pot>> GetAllPotsAsync(CancellationToken cancellationToken = default)
	{
		PotRepository potRepository = new(DbContext);

		return potRepository.GetAllAsync(cancellationToken)
			.ToListAsync();
	}
}