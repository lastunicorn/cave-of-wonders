using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess;

/// <summary>
/// Base class for the JSON storage back doors. Owns the back-door session: a <c>Database</c>
/// opened over the same temporary directory as the SUT's session, but created independently via
/// <see cref="JsonTempDatabase.CreateSessionAsync"/>, so seeding and inspection never go through the instance
/// handed to the Act phase. One <see cref="OpenAsync"/>/<see cref="CloseAsync"/> cycle is expected per
/// Arrange/Assert phase; <see cref="CloseAsync"/> saves the session, flushing seeded data to disk.
/// </summary>
internal abstract class JsonStorageBackDoorBase
{
	private readonly JsonTempDatabase jsonTempDatabase;

	protected Database Database { get; private set; }

	protected JsonStorageBackDoorBase(JsonTempDatabase jsonTempDatabase)
	{
		this.jsonTempDatabase = jsonTempDatabase ?? throw new ArgumentNullException(nameof(jsonTempDatabase));
	}

	public async Task OpenAsync(CancellationToken cancellationToken = default)
	{
		Database = await jsonTempDatabase.CreateSessionAsync(cancellationToken);
	}

	public async Task CloseAsync(CancellationToken cancellationToken = default)
	{
		await Database.SaveAsync(cancellationToken);
		Database = null;
	}
}