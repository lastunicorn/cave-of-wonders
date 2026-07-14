namespace DustInTheWind.CaveOfWonders.Tests.Utils;

/// <summary>
/// Knows how to stand up, tear down, and reset one concrete adapter's implementation of repository interface
/// <typeparamref name="T"/>, so that a <see cref="GenericTest{TSut}"/> can exercise the same test body against
/// whichever adapter instance is injected at runtime.
/// </summary>
/// <remarks>
/// A single provider instance is reused across every phase of one <see cref="GenericTest{TSut}"/> run: <see cref="CreateAsync"/>
/// is called once per phase (Arrange/Act/Assert) to obtain a fresh <typeparamref name="T"/> backed by the same
/// underlying storage, and <see cref="ReleaseAsync"/> is called right after to persist and release it. Implementations
/// that need the underlying storage handle (e.g. to save changes in <see cref="ReleaseAsync"/>) should keep it in an
/// instance field set during <see cref="CreateAsync"/>, since only the SUT instance is passed back to
/// <see cref="ReleaseAsync"/>. <see cref="Reset"/> runs once at the very end, regardless of outcome, to remove any
/// temporary storage created for the run.
/// </remarks>
public interface ISutProvider<T>
{
    Task<T> CreateAsync();

    Task ReleaseAsync(T sut);

    void Reset();
}
