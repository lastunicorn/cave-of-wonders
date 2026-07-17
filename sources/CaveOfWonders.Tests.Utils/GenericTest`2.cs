namespace DustInTheWind.CaveOfWonders.Tests.Utils;

/// <summary>
/// A fluent builder that runs an Arrange/Act/Assert integration test in which the phase/access-path rule is enforced
/// by the compiler: the Arrange and Assert lambdas receive a back door (<typeparamref name="TBackDoor"/>)
/// that seeds and inspects the underlying storage directly, while only the Act lambda receives the system under test
/// (<typeparamref name="TSut"/>). Arrange cannot touch the SUT and Act cannot touch the back door, so a mutation test
/// verifies what was actually persisted instead of round-tripping through the SUT's own read path.
/// </summary>
/// <remarks>
/// <para>
/// Each phase is configured independently and executed only once <see cref="ExecuteAsync"/> is called. Every phase
/// obtains and releases its own session via the injected <see cref="ITestEnvironment{TSut,TBackDoor}"/>: the back-door
/// session (<see cref="ITestEnvironment{TSut,TBackDoor}.CreateBackDoorAsync"/>/<see cref="ITestEnvironment{TSut,TBackDoor}.CloseBackDoorAsync"/>)
/// around Arrange and Assert, and the SUT session (<see cref="ITestEnvironment{TSut,TBackDoor}.CreateSutAsync"/>/<see cref="ITestEnvironment{TSut,TBackDoor}.CloseSutAsync"/>)
/// around Act. For a SUT backed by persistent storage, this forces data to be actually persisted to and reloaded
/// between phases.
/// </para>
/// <para>
/// Both synchronous and asynchronous overloads are provided for each phase, and each phase may be set at most once
/// (calling it twice throws <see cref="InvalidOperationException"/>). All phases are optional, so a test can omit
/// Arrange (e.g. to act on an empty/initial state) or Assert (e.g. when <see cref="AssertThrow"/> handles the
/// outcome). Query tests are expected to assert on Act's return value, carried into Assert through the context bag,
/// rather than on storage.
/// </para>
/// <para>
/// A <see cref="GenericTestContext"/> instance is threaded through every phase as a dynamic bag, letting a test
/// pass values (e.g. generated ids or results) from Arrange into Act and from Act into Assert without declaring
/// dedicated fields on the test class.
/// </para>
/// <para>
/// The environment's <see cref="IDisposable.Dispose"/> and <see cref="ITestEnvironment{TSut,TBackDoor}.ResetAsync"/>
/// are both always called in a <c>finally</c> block, so a resource left open by a failed phase is released and the
/// external resource created for the run is reset/removed, even if one of the phases throws.
/// </para>
/// </remarks>
public class GenericTest<TSut, TBackDoor>
{
	private readonly ITestEnvironment<TSut, TBackDoor> environment;

	private Func<TBackDoor, dynamic, Task> arrangeAction1;
	private Action<TBackDoor, dynamic> arrangeAction2;

	private Func<TSut, dynamic, Task> actAction1;
	private Action<TSut, dynamic> actAction2;

	private Func<TBackDoor, dynamic, Task> assertAction1;
	private Action<TBackDoor, dynamic> assertAction2;

	private Action<Exception> errorHandler;

	public GenericTest(ITestEnvironment<TSut, TBackDoor> environment)
	{
		this.environment = environment ?? throw new ArgumentNullException(nameof(environment));
	}

	public GenericTest<TSut, TBackDoor> Arrange(Func<TBackDoor, dynamic, Task> action)
	{
		if (arrangeAction1 != null || arrangeAction2 != null)
			throw new InvalidOperationException("Arrange can only be called once.");

		arrangeAction1 = action;
		return this;
	}

	public GenericTest<TSut, TBackDoor> Arrange(Action<TBackDoor, dynamic> action)
	{
		if (arrangeAction1 != null || arrangeAction2 != null)
			throw new InvalidOperationException("Arrange can only be called once.");

		arrangeAction2 = action;
		return this;
	}

	public GenericTest<TSut, TBackDoor> Act(Func<TSut, dynamic, Task> action)
	{
		if (actAction1 != null || actAction2 != null)
			throw new InvalidOperationException("Act can only be called once.");

		actAction1 = action;
		return this;
	}

	public GenericTest<TSut, TBackDoor> Act(Action<TSut, dynamic> action)
	{
		if (actAction1 != null || actAction2 != null)
			throw new InvalidOperationException("Act can only be called once.");

		actAction2 = action;
		return this;
	}

	public GenericTest<TSut, TBackDoor> Assert(Func<TBackDoor, dynamic, Task> action)
	{
		if (assertAction1 != null || assertAction2 != null)
			throw new InvalidOperationException("Assert can only be called once.");

		assertAction1 = action;
		return this;
	}

	public GenericTest<TSut, TBackDoor> Assert(Action<TBackDoor, dynamic> action)
	{
		if (assertAction1 != null || assertAction2 != null)
			throw new InvalidOperationException("Assert can only be called once.");

		assertAction2 = action;
		return this;
	}

	public GenericTest<TSut, TBackDoor> AssertThrow(Action<Exception> errorHandler)
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
				await environment.CreateBackDoorAsync(cancellationToken);
				await arrangeAction1(environment.BackDoor, context);
				await environment.CloseBackDoorAsync(cancellationToken);
			}

			if (arrangeAction2 != null)
			{
				await environment.CreateBackDoorAsync(cancellationToken);
				arrangeAction2(environment.BackDoor, context);
				await environment.CloseBackDoorAsync(cancellationToken);
			}

			try
			{
				if (actAction1 != null)
				{
					await environment.CreateSutAsync(cancellationToken);
					await actAction1(environment.Sut, context);
					await environment.CloseSutAsync(cancellationToken);
				}

				if (actAction2 != null)
				{
					await environment.CreateSutAsync(cancellationToken);
					actAction2(environment.Sut, context);
					await environment.CloseSutAsync(cancellationToken);
				}
			}
			catch (Exception ex)
			{
				if (errorHandler != null)
				{
					errorHandler(ex);
					return;
				}
				else
				{
					throw;
				}
			}

			if (assertAction1 != null)
			{
				await environment.CreateBackDoorAsync(cancellationToken);
				await assertAction1(environment.BackDoor, context);
				await environment.CloseBackDoorAsync(cancellationToken);
			}

			if (assertAction2 != null)
			{
				await environment.CreateBackDoorAsync(cancellationToken);
				assertAction2(environment.BackDoor, context);
				await environment.CloseBackDoorAsync(cancellationToken);
			}

			if (errorHandler != null)
				errorHandler(null);
		}
		finally
		{
			environment.Dispose();
			await environment.ResetAsync(cancellationToken);
		}
	}
}