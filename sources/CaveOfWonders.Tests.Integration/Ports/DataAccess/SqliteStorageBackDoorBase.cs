using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess;

/// <summary>
/// Base class for the SQLite storage back doors. Owns the back-door session: a
/// <see cref="CaveOfWondersDbContext"/> opened over the same temporary database file as the SUT's session, but
/// created independently via <see cref="SqliteTempDatabase.CreateSessionAsync"/>, so seeding and inspection never
/// go through the instance handed to the Act phase. One <see cref="OpenAsync"/>/<see cref="CloseAsync"/> cycle is
/// expected per Arrange/Assert phase; <see cref="CloseAsync"/> saves the session, flushing seeded data to disk.
/// </summary>
internal abstract class SqliteStorageBackDoorBase
{
	private readonly SqliteTempDatabase sqliteTempDatabase;

	protected CaveOfWondersDbContext DbContext { get; private set; }

	protected SqliteStorageBackDoorBase(SqliteTempDatabase sqliteTempDatabase)
	{
		this.sqliteTempDatabase = sqliteTempDatabase ?? throw new ArgumentNullException(nameof(sqliteTempDatabase));
	}

	public async Task OpenAsync(CancellationToken cancellationToken = default)
	{
		DbContext = await sqliteTempDatabase.CreateSessionAsync(cancellationToken);
	}

	public async Task CloseAsync(CancellationToken cancellationToken = default)
	{
		await DbContext.SaveChangesAsync(cancellationToken);

		await DbContext.DisposeAsync();
		DbContext = null;
	}
}
