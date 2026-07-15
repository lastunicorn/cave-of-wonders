namespace DustInTheWind.CaveOfWonders.Tests.Utils;

/// <summary>
/// Represents the system under test (SUT): one concrete implementation of interface <typeparamref name="T"/>,
/// exposed through <see cref="Sut"/>, so that a <see cref="GenericTest{TSut}"/> can exercise the same test body
/// against whichever implementation is injected at runtime (e.g. a repository backed by JSON, LiteDb, or SQLite).
/// Since an <see cref="ISutFixture{T}"/> owns whatever external resource (e.g. a temporary database file) backs
/// <typeparamref name="T"/>, it manages that resource's entire lifecycle, including resetting/removing it via
/// <see cref="ResetAsync"/> and <see cref="IDisposable.Dispose"/>.
/// </summary>
/// <remarks>
/// A single <see cref="ISutFixture{T}"/> instance is reused across every phase of one <see cref="GenericTest{TSut}"/> run:
/// <see cref="CreateSutAsync"/> is called once per phase (Arrange/Act/Assert) to stand up a fresh <typeparamref name="T"/>
/// backed by the same underlying state, setting <see cref="Sut"/> to it, and <see cref="ReleaseSutAsync"/> is called
/// right after to persist and release it. <see cref="IDisposable.Dispose"/> and <see cref="ResetAsync"/> are both called exactly once,
/// at the very end, regardless of outcome: <see cref="IDisposable.Dispose"/> releases any resource left open because a phase's action
/// threw before its <see cref="ReleaseSutAsync"/> could run and resets the external resource synchronously, while
/// <see cref="ResetAsync"/> is the equivalent async entry point, for callers that can await it directly instead of relying
/// on <see cref="IDisposable.Dispose"/>.
/// </remarks>
public interface ISutFixture<out T> : IDisposable
{
	T Sut { get; }

	Task CreateSutAsync(CancellationToken cancellationToken = default);

	Task ReleaseSutAsync(CancellationToken cancellationToken = default);

	Task ResetAsync(CancellationToken cancellationToken = default);
}