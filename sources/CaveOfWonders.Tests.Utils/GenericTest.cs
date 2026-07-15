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
/// <see cref="ISutFixture{T}.CreateSutAsync"/>/<see cref="ISutFixture{T}.ReleaseSutAsync"/> members, exposed meanwhile through
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
	private readonly ISutFixture<TSut> sutFixture;

	private Func<TSut, dynamic, Task> arrangeAction1;
	private Action<TSut, dynamic> arrangeAction2;

	private Func<TSut, dynamic, Task> actAction1;
	private Action<TSut, dynamic> actAction2;

	private Func<TSut, dynamic, Task> assertAction1;
	private Action<TSut, dynamic> assertAction2;
	
	private Action<Exception> errorHandler;

	public GenericTest(ISutFixture<TSut> sutFixture)
	{
		this.sutFixture = sutFixture ?? throw new ArgumentNullException(nameof(sutFixture));
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

	public GenericTest<TSut> AssertThrow(Action<Exception> errorHandler)
	{
		this.errorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
		return this;
	}

	public async Task ExecuteAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			GenericTestContext context = new();

			if (arrangeAction1 != null)
			{
				await sutFixture.CreateSutAsync(cancellationToken);
				await arrangeAction1(sutFixture.Instance, context);
				await sutFixture.ReleaseSutAsync(cancellationToken);
			}

			if (arrangeAction2 != null)
			{
				await sutFixture.CreateSutAsync(cancellationToken);
				arrangeAction2(sutFixture.Instance, context);
				await sutFixture.ReleaseSutAsync(cancellationToken);
			}

			if (actAction1 != null)
			{
				await sutFixture.CreateSutAsync(cancellationToken);
				await actAction1(sutFixture.Instance, context);
				await sutFixture.ReleaseSutAsync(cancellationToken);
			}

			if (actAction2 != null)
			{
				await sutFixture.CreateSutAsync(cancellationToken);
				actAction2(sutFixture.Instance, context);
				await sutFixture.ReleaseSutAsync(cancellationToken);
			}

			if (assertAction1 != null)
			{
				await sutFixture.CreateSutAsync(cancellationToken);
				await assertAction1(sutFixture.Instance, context);
				await sutFixture.ReleaseSutAsync(cancellationToken);
			}

			if (assertAction2 != null)
			{
				await sutFixture.CreateSutAsync(cancellationToken);
				assertAction2(sutFixture.Instance, context);
				await sutFixture.ReleaseSutAsync(cancellationToken);
			}
		}
		catch (Exception ex)
		{
			if (errorHandler != null)
				errorHandler(ex);
			else
				throw;
		}
		finally
		{
			sutFixture.Dispose();
			await sutFixture.ResetAsync(cancellationToken);
		}
	}
}

public static class GenericTest
{
	public static GenericTest<TSut> Create<TSut>(ISutFixture<TSut> sutFixture)
	{
		return new GenericTest<TSut>(sutFixture);
	}
}