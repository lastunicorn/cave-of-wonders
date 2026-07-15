using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.Repositories;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures.JsonFixtures;

internal class JsonCpiRepositoryFixture : ISutFixture<ICpiRepository>
{
	private readonly JsonTempDatabase jsonTempDatabase = new();

	public ICpiRepository Sut { get; private set; }

	public async Task CreateSutAsync(CancellationToken cancellationToken = default)
	{
		await jsonTempDatabase.OpenAsync(cancellationToken);
		Sut = new CpiRepository(jsonTempDatabase.Database);
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
