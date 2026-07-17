using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.TestEnvironments.LiteDbEnvironment;

/// <summary>
/// Base class for the LiteDb storage gateways. Owns the gateway's back-door session: a <see cref="DbContext"/>
/// opened over the same temporary file as the SUT's session, but created independently via
/// <see cref="LiteDbTempDatabase.CreateSessionAsync"/>, so seeding and inspection never go through the instance
/// handed to the Act phase. One <see cref="OpenAsync"/>/<see cref="CloseAsync"/> cycle is expected per
/// Arrange/Assert phase. Unlike the JSON and SQLite adapters, LiteDb writes are not buffered in memory, so
/// <see cref="CloseAsync"/> only needs to dispose the session.
/// </summary>
internal abstract class LiteDbStorageGateway
{
	private readonly LiteDbTempDatabase liteDbTempDatabase;

	protected DbContext DbContext { get; private set; }

	protected LiteDbStorageGateway(LiteDbTempDatabase liteDbTempDatabase)
	{
		this.liteDbTempDatabase = liteDbTempDatabase ?? throw new ArgumentNullException(nameof(liteDbTempDatabase));
	}

	public async Task OpenAsync(CancellationToken cancellationToken = default)
	{
		DbContext = await liteDbTempDatabase.CreateSessionAsync(cancellationToken);
	}

	public Task CloseAsync(CancellationToken cancellationToken = default)
	{
		DbContext.Dispose();
		DbContext = null;

		return Task.CompletedTask;
	}
}
