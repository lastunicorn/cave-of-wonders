using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Infrastructure;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.Repositories;

public class CpiRepository : ICpiRepository
{
	private readonly Database database;

	public CpiRepository(Database database)
	{
		this.database = database ?? throw new ArgumentNullException(nameof(database));
	}

	public IAsyncEnumerable<Cpi> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return database.CpiRecords.ToAsyncEnumerable(cancellationToken);
	}

	public Task<Cpi> GetByYear(int year)
	{
		Cpi cpi = database.CpiRecords
			.FirstOrDefault(x => x.Year == year);

		return Task.FromResult(cpi);
	}

	public void Add(Cpi cpi)
	{
		if (cpi == null) throw new ArgumentNullException(nameof(cpi));

		bool alreadyExists = database.CpiRecords.Any(x => x.Year == cpi.Year);

		if (alreadyExists)
			throw new ArgumentException($"A CPI record for year '{cpi.Year}' already exists.", nameof(cpi));

		database.CpiRecords.Add(cpi);
	}
}