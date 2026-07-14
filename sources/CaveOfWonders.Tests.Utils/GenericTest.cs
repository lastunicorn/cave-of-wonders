namespace DustInTheWind.CaveOfWonders.Tests.Utils;

/// <summary>
/// A fluent builder that runs an Arrange/Act/Assert integration test against a system under test (SUT), addressed
/// only through its interface (<typeparamref name="TSut"/>), so the same test can be executed against any concrete
/// implementation (e.g. a repository backed by JSON, LiteDb, or SQLite) supplied via an <see cref="ISutFixture{T}"/>
/// at runtime.
/// </summary>
/// <remarks>
/// <para>
/// Each phase (<see cref="Arrange(System.Action{TSut,dynamic})"/>, <see cref="Act(System.Action{TSut,dynamic})"/>,
/// <see cref="Assert(System.Action{TSut,dynamic})"/>) is configured independently and executed only once
/// <see cref="ExecuteAsync"/> is called.
/// </para>
/// <para>
/// Every phase obtains and releases its own SUT instance (via the injected <see cref="ISutFixture{T}"/>'s
/// <see cref="ISutFixture{T}.CreateInstanceAsync"/>/<see cref="ISutFixture{T}.ReleaseInstanceAsync"/> members, exposed meanwhile through
/// <see cref="ISutFixture{T}.Instance"/>), rather than sharing one instance across the whole test. For a SUT backed by
/// persistent storage, this forces data to be actually persisted to and reloaded between phases, so the test
/// exercises real persistence instead of asserting against an in-memory object graph that was never saved.
/// </para>
/// <para>
/// Both synchronous and asynchronous overloads are provided for each phase, and each phase may be set at most once
/// (calling it twice throws <see cref="InvalidOperationException"/>). All phases are optional, so a test can omit
/// Arrange (e.g. to assert on an empty/initial SUT state) or Assert (e.g. to only verify no exception is thrown
/// during Act).
/// </para>
/// <para>
/// A <see cref="GenericTestContext"/> instance is threaded through every phase as a dynamic bag, letting a test
/// pass values (e.g. generated ids or results) from Arrange into Act and from Act into Assert without declaring
/// dedicated fields on the test class.
/// </para>
/// <para>
/// The provider's <see cref="IDisposable.Dispose"/> and <see cref="ISutFixture{T}.ResetAsync"/> are both always
/// called in a <c>finally</c> block, so a resource left open by a failed phase is released and the external
/// resource created for the run is reset/removed, even if one of the phases throws.
/// </para>
/// </remarks>
public class GenericTest<TSut>
{
	private readonly ISutFixture<TSut> sut;

	private Func<TSut, dynamic, Task> arrangeAction1;
	private Action<TSut, dynamic> arrangeAction2;

	private Func<TSut, dynamic, Task> actAction1;
	private Action<TSut, dynamic> actAction2;

	private Func<TSut, dynamic, Task> assertAction1;
	private Action<TSut, dynamic> assertAction2;

	public GenericTest(ISutFixture<TSut> sut)
	{
		this.sut = sut ?? throw new ArgumentNullException(nameof(sut));
	}

	public GenericTest<TSut> Arrange(Func<TSut, dynamic, Task> action)
	{
		if (arrangeAction1 != null || arrangeAction2 != null)
			throw new InvalidOperationException("Arrange can only be called once.");

		arrangeAction1 = action;
		return this;
	}

	public GenericTest<TSut> Arrange(Action<TSut, dynamic> action)
	{
		if (arrangeAction1 != null || arrangeAction2 != null)
			throw new InvalidOperationException("Arrange can only be called once.");

		arrangeAction2 = action;
		return this;
	}

	public GenericTest<TSut> Act(Func<TSut, dynamic, Task> action)
	{
		if (actAction1 != null || actAction2 != null)
			throw new InvalidOperationException("Act can only be called once.");

		actAction1 = action;
		return this;
	}

	public GenericTest<TSut> Act(Action<TSut, dynamic> action)
	{
		if (actAction1 != null || actAction2 != null)
			throw new InvalidOperationException("Act can only be called once.");

		actAction2 = action;
		return this;
	}

	public GenericTest<TSut> Assert(Func<TSut, dynamic, Task> action)
	{
		if (assertAction1 != null || assertAction2 != null)
			throw new InvalidOperationException("Assert can only be called once.");

		assertAction1 = action;
		return this;
	}

	public GenericTest<TSut> Assert(Action<TSut, dynamic> action)
	{
		if (assertAction1 != null || assertAction2 != null)
			throw new InvalidOperationException("Assert can only be called once.");

		assertAction2 = action;
		return this;
	}

	public async Task ExecuteAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			GenericTestContext context = new();

			if (arrangeAction1 != null)
			{
				await sut.CreateInstanceAsync(cancellationToken);
				await arrangeAction1(sut.Instance, context);
				await sut.ReleaseInstanceAsync(cancellationToken);
			}

			if (arrangeAction2 != null)
			{
				await sut.CreateInstanceAsync(cancellationToken);
				arrangeAction2(sut.Instance, context);
				await sut.ReleaseInstanceAsync(cancellationToken);
			}

			if (actAction1 != null)
			{
				await sut.CreateInstanceAsync(cancellationToken);
				await actAction1(sut.Instance, context);
				await sut.ReleaseInstanceAsync(cancellationToken);
			}

			if (actAction2 != null)
			{
				await sut.CreateInstanceAsync(cancellationToken);
				actAction2(sut.Instance, context);
				await sut.ReleaseInstanceAsync(cancellationToken);
			}

			if (assertAction1 != null)
			{
				await sut.CreateInstanceAsync(cancellationToken);
				await assertAction1(sut.Instance, context);
				await sut.ReleaseInstanceAsync(cancellationToken);
			}

			if (assertAction2 != null)
			{
				await sut.CreateInstanceAsync(cancellationToken);
				assertAction2(sut.Instance, context);
				await sut.ReleaseInstanceAsync(cancellationToken);
			}
		}
		finally
		{
			sut.Dispose();
			await sut.ResetAsync(cancellationToken);
		}
	}
}