namespace DustInTheWind.CaveOfWonders.Tests.Utils;

/// <summary>
/// A fluent builder that runs an Arrange/Act/Assert integration test against a real database instance.
/// </summary>
/// <remarks>
/// <para>
/// Each phase (<see cref="Arrange(System.Action{TDb,dynamic})"/>, <see cref="Act(System.Action{TDb,dynamic})"/>,
/// <see cref="Assert(System.Action{TDb,dynamic})"/>) is configured independently and executed only once
/// <see cref="Execute"/> is called.
/// </para>
/// <para>
/// Every phase opens and closes its own database instance (via the abstract <c>OpenDatabaseAsync</c>/<c>CloseDatabaseAsync</c>
/// members implemented by a derived class), rather than sharing one instance across the whole test. This forces data to be
/// actually persisted to and reloaded from disk between phases, so the test exercises real persistence.
/// </para>
/// <para>
/// Both synchronous and asynchronous overloads are provided for each phase, and each phase may be set at most once
/// (calling it twice throws <see cref="InvalidOperationException"/>). All phases are optional, so a test can omit
/// Arrange (e.g. to assert on an empty database) or Assert (e.g. to only verify no exception is thrown during Act).
/// </para>
/// <para>
/// A <see cref="DatabaseTestContext"/> instance is threaded through every phase as a dynamic bag, letting a test pass
/// values (e.g. generated ids or results) from Arrange into Act and from Act into Assert without declaring dedicated
/// fields on the test class.
/// </para>
/// <para>
/// The database is always cleaned up via <c>ResetDatabase</c> in a <c>finally</c> block, so temporary files are removed
/// even if one of the phases throws.
/// </para>
/// </remarks>
public abstract class DatabaseTest<TDb>
{
    private Func<TDb, dynamic, Task> arrangeAction1;
    private Action<TDb, dynamic> arrangeAction2;

    private Func<TDb, dynamic, Task> actAction1;
    private Action<TDb, dynamic> actAction2;

    private Func<TDb, dynamic, Task> assertAction1;
    private Action<TDb, dynamic> assertAction2;

    protected abstract Task<TDb> OpenDatabaseAsync();

    protected abstract Task CloseDatabaseAsync(TDb database);

    protected abstract void ResetDatabase();

    public DatabaseTest<TDb> Arrange(Func<TDb, dynamic, Task> action)
    {
        if (arrangeAction1 != null || arrangeAction2 != null)
            throw new InvalidOperationException("Arrange can only be called once.");

        arrangeAction1 = action;
        return this;
    }

    public DatabaseTest<TDb> Arrange(Action<TDb, dynamic> action)
    {
        if (arrangeAction1 != null || arrangeAction2 != null)
            throw new InvalidOperationException("Arrange can only be called once.");

        arrangeAction2 = action;
        return this;
    }

    public DatabaseTest<TDb> Act(Func<TDb, dynamic, Task> action)
    {
        if (actAction1 != null || actAction2 != null)
            throw new InvalidOperationException("Act can only be called once.");

        actAction1 = action;
        return this;
    }

    public DatabaseTest<TDb> Act(Action<TDb, dynamic> action)
    {
        if (actAction1 != null || actAction2 != null)
            throw new InvalidOperationException("Act can only be called once.");

        actAction2 = action;
        return this;
    }

    public DatabaseTest<TDb> Assert(Func<TDb, dynamic, Task> action)
    {
        if (assertAction1 != null || assertAction2 != null)
            throw new InvalidOperationException("Assert can only be called once.");

        assertAction1 = action;
        return this;
    }

    public DatabaseTest<TDb> Assert(Action<TDb, dynamic> action)
    {
        if (assertAction1 != null || assertAction2 != null)
            throw new InvalidOperationException("Assert can only be called once.");

        assertAction2 = action;
        return this;
    }

    public async Task Execute()
    {
        try
        {
            DatabaseTestContext context = new();

            if (arrangeAction1 != null)
            {
                TDb db = await OpenDatabaseAsync();
                await arrangeAction1(db, context);
                await CloseDatabaseAsync(db);
            }

            if (arrangeAction2 != null)
            {
                TDb db = await OpenDatabaseAsync();
                arrangeAction2(db, context);
                await CloseDatabaseAsync(db);
            }

            if (actAction1 != null)
            {
                TDb db = await OpenDatabaseAsync();
                await actAction1(db, context);
                await CloseDatabaseAsync(db);
            }

            if (actAction2 != null)
            {
                TDb db = await OpenDatabaseAsync();
                actAction2(db, context);
                await CloseDatabaseAsync(db);
            }

            if (assertAction1 != null)
            {
                TDb db = await OpenDatabaseAsync();
                await assertAction1(db, context);
                await CloseDatabaseAsync(db);
            }

            if (assertAction2 != null)
            {
                TDb db = await OpenDatabaseAsync();
                assertAction2(db, context);
                await CloseDatabaseAsync(db);
            }
        }
        finally
        {
            ResetDatabase();
        }
    }
}
