using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.Repositories;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures.JsonFixtures;

internal class JsonPotRepositoryFixture : ISutFixture<IPotRepository>
{
	private readonly JsonTempDatabase jsonTempDatabase = new();

	public IPotRepository Sut { get; private set; }

	public async Task CreateSutAsync(CancellationToken cancellationToken = default)
	{
		await jsonTempDatabase.OpenAsync(cancellationToken);
		Sut = new PotRepository(jsonTempDatabase.Database);
	}

	public async Task ReleaseSutAsync(CancellationToken cancellationToken = default)
	{
		await jsonTempDatabase.CloseAsync(cancellationToken);
		Sut = null;
	}

	public Task ResetAsync(CancellationToken cancellationToken = default)
	{
		jsonTempDatabase.Dispose();
		Sut = null;

		return Task.CompletedTask;
	}

	public void Dispose()
	{
		jsonTempDatabase.Dispose();
		Sut = null;
	}

	public override string ToString()
	{
		return "Json";
	}
}
