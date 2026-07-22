using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotSnapshotRepositoryTests.TestEnvironments;

/// <summary>
/// Back-door access to the storage behind an <c>IPotSnapshotRepository</c> under test. Used by Arrange to seed state
/// and by Assert to inspect persisted state, bypassing the SUT's own read/write paths. Each implementation works
/// against the same temporary resource as the SUT, but through its own, independently opened session.
/// </summary>
public interface ITestBackDoor
{
	Task SeedPotsAsync(IEnumerable<Pot> pots, CancellationToken cancellationToken = default);

	Task<List<PotSnapshot>> GetSnapshotsByPotIdAsync(Guid potId, CancellationToken cancellationToken = default);
}
