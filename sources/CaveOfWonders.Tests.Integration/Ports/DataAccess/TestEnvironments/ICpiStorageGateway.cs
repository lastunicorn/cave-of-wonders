using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.TestEnvironments;

/// <summary>
/// Back-door access to the storage behind an <c>ICpiRepository</c> under test. Used by Arrange to seed state and by
/// Assert to inspect persisted state, bypassing the SUT's own read/write paths. Each implementation works against the
/// same temporary resource as the SUT, but through its own, independently opened session.
/// </summary>
public interface ICpiStorageGateway
{
	Task SeedCpisAsync(IEnumerable<Cpi> cpis, CancellationToken cancellationToken = default);

	Task<List<Cpi>> GetAllCpisAsync(CancellationToken cancellationToken = default);
}
