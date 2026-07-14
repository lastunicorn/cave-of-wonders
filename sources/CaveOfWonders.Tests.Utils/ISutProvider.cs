namespace DustInTheWind.CaveOfWonders.Tests.Utils;

/// <summary>
/// Knows how to stand up, tear down, and reset one concrete implementation of interface <typeparamref name="T"/>
/// (the system under test), so that a <see cref="GenericTest{TSut}"/> can exercise the same test body against
/// whichever implementation is injected at runtime (e.g. a repository backed by JSON, LiteDb, or SQLite).
/// </summary>
/// <remarks>
/// A single provider instance is reused across every phase of one <see cref="GenericTest{TSut}"/> run: <see cref="CreateAsync"/>
/// is called once per phase (Arrange/Act/Assert) to obtain a fresh <typeparamref name="T"/> backed by the same
/// underlying state, and <see cref="ReleaseAsync"/> is called right after to persist and release it. Implementations
/// that need the underlying state handle (e.g. to save changes in <see cref="ReleaseAsync"/>) should keep it in an
/// instance field set during <see cref="CreateAsync"/>, since only the SUT instance is passed back to
/// <see cref="ReleaseAsync"/>. <see cref="Reset"/> runs once at the very end, regardless of outcome, to remove any
/// temporary state created for the run.
/// </remarks>
public interface ISutProvider<T>
{
    Task<T> CreateAsync(CancellationToken cancellationToken = default);

    Task ReleaseAsync(T sut, CancellationToken cancellationToken = default);

    void Reset();
}
