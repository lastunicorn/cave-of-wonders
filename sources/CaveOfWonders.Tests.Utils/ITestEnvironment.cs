namespace DustInTheWind.CaveOfWonders.Tests.Utils;

/// <summary>
/// The per-adapter strategy consumed by a <see cref="GenericTest{TSut,TBackDoor}"/>. It owns the temporary external
/// resource (e.g. a temporary database) backing one test run and can stand up two independent access paths to it on
/// demand: the SUT (<typeparamref name="TSut"/>), exercised by the Act phase, and a back door
/// (<typeparamref name="TBackDoor"/>), used by the Arrange phase to seed state and by the Assert phase to inspect
/// persisted state without going through the SUT's own read/write paths (Back Door Manipulation).
/// </summary>
/// <remarks>
/// A single instance is reused across every phase of one test run: <see cref="CreateBackDoorAsync"/>/<see cref="CloseBackDoorAsync"/>
/// are called around Arrange and Assert, and <see cref="CreateSutAsync"/>/<see cref="CloseSutAsync"/> around Act.
/// Each Create must open a fresh session over the same persisted state — never reuse the other access path's
/// session — so data is actually persisted and reloaded between phases. <see cref="IDisposable.Dispose"/> and
/// <see cref="ResetAsync"/> are both called exactly once, at the very end, regardless of outcome:
/// <see cref="IDisposable.Dispose"/> releases any session left open because a phase's action threw before its
/// Release could run, and <see cref="ResetAsync"/> removes the external resource created for the run.
/// </remarks>
public interface ITestEnvironment<out TSut, out TBackDoor> : IDisposable
{
	TSut Sut { get; }

	TBackDoor BackDoor { get; }

	/// <summary>
	/// Opens a fresh SUT session (used once per Act phase).
	/// </summary>
	Task CreateSutAsync(CancellationToken cancellationToken = default);

	Task CloseSutAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Opens a fresh back-door session (used once per Arrange/Assert phase).
	/// </summary>
	Task CreateBackDoorAsync(CancellationToken cancellationToken = default);

	Task CloseBackDoorAsync(CancellationToken cancellationToken = default);

	Task ResetAsync(CancellationToken cancellationToken = default);
}