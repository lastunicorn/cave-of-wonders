namespace DustInTheWind.CaveOfWonders.Tests.Utils;

public abstract class DatabaseTest<TDb>
{
    protected readonly string DbPath;

    private Func<TDb, dynamic, Task> arrangeAction1;
    private Action<TDb, dynamic> arrangeAction2;

    private Func<TDb, dynamic, Task> actAction1;
    private Action<TDb, dynamic> actAction2;

    private Func<TDb, dynamic, Task> assertAction1;
    private Action<TDb, dynamic> assertAction2;

    protected DatabaseTest(string dbPath)
    {
        DbPath = dbPath;
    }

    protected abstract Task<TDb> OpenDatabaseAsync();

    protected abstract Task CloseDatabaseAsync(TDb database);

    protected virtual void RemoveDatabase()
    {
        if (Directory.Exists(DbPath))
            Directory.Delete(DbPath, true);
    }

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
            RemoveDatabase();
        }
    }
}
