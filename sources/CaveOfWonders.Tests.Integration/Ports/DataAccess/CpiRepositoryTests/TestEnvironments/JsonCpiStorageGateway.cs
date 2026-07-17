using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.CpiRepositoryTests.TestEnvironments;

internal class JsonCpiStorageGateway : JsonStorageGatewayBase, ICpiStorageGateway
{
	public JsonCpiStorageGateway(JsonTempDatabase jsonTempDatabase)
		: base(jsonTempDatabase)
	{
	}

	public Task SeedCpisAsync(IEnumerable<Cpi> cpis, CancellationToken cancellationToken = default)
	{
		Database.CpiRecords.AddRange(cpis);
		return Task.CompletedTask;
	}

	public Task<List<Cpi>> GetAllCpisAsync(CancellationToken cancellationToken = default)
	{
		List<Cpi> cpis = Database.CpiRecords.ToList();
		return Task.FromResult(cpis);
	}
}
