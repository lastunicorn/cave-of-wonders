namespace DustInTheWind.CaveOfWonders.Tests.Utils;

/// <summary>
/// Represents the system under test (SUT): one concrete implementation of interface <typeparamref name="T"/>,
/// exposed through <see cref="Instance"/>, so that a <see cref="GenericTest{TSut}"/> can exercise the same test body
/// against whichever implementation is injected at runtime (e.g. a repository backed by JSON, LiteDb, or SQLite).
/// </summary>
/// <remarks>
/// A single <see cref="ISut{T}"/> instance is reused across every phase of one <see cref="GenericTest{TSut}"/> run:
/// <see cref="CreateInstanceAsync"/> is called once per phase (Arrange/Act/Assert) to stand up a fresh <typeparamref name="T"/>
/// backed by the same underlying state, setting <see cref="Instance"/> to it, and <see cref="ReleaseInstanceAsync"/> is called
/// right after to persist and release it. <see cref="ResetAsync"/> runs once at the very end, regardless of outcome, to
/// remove any temporary state created for the run.
/// </remarks>
public interface ISut<out T>
{
    T Instance { get; }

    Task CreateInstanceAsync(CancellationToken cancellationToken = default);

    Task ReleaseInstanceAsync(CancellationToken cancellationToken = default);

    Task ResetAsync(CancellationToken cancellationToken = default);
}
