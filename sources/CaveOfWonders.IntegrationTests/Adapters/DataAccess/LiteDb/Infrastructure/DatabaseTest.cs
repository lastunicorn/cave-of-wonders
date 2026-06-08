using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;

namespace DustInTheWind.CaveOfWonders.IntegrationTests.Adapters.DataAccess.LiteDb.Infrastructure;

internal class DatabaseTest
{
    private readonly string dbDirectoryPath;

    private Func<DbContext, dynamic, Task> arrangeAction1;
    private Action<DbContext, dynamic> arrangeAction2;

    private Func<DbContext, dynamic, Task> actAction1;
    private Action<DbContext, dynamic> actAction2;

    private Func<DbContext, dynamic, Task> assertAction1;
    private Action<DbContext, dynamic> assertAction2;

    public static DatabaseTest Create()
    {
        return new DatabaseTest();
    }

    private DatabaseTest()
    {
        dbDirectoryPath = Path.Combine(Path.GetTempPath(), $"test-dbContext-{Guid.NewGuid()}");
    }

    public DatabaseTest Arrange(Func<DbContext, dynamic, Task> action)
    {
        if (arrangeAction1 != null || arrangeAction2 != null)
            throw new InvalidOperationException("Arrange can only be called once.");

        arrangeAction1 = action;
        return this;
    }

    public DatabaseTest Arrange(Action<DbContext, dynamic> action)
    {
        if (arrangeAction1 != null || arrangeAction2 != null)
            throw new InvalidOperationException("Arrange can only be called once.");

        arrangeAction2 = action;
        return this;
    }

    public DatabaseTest Act(Func<DbContext, dynamic, Task> action)
    {
        if (actAction1 != null || actAction2 != null)
            throw new InvalidOperationException("Act can only be called once.");

        actAction1 = action;
        return this;
    }

    public DatabaseTest Act(Action<DbContext, dynamic> action)
    {
        if (actAction1 != null || actAction2 != null)
            throw new InvalidOperationException("Act can only be called once.");

        actAction2 = action;
        return this;
    }

    public DatabaseTest Assert(Func<DbContext, dynamic, Task> action)
    {
        if (assertAction1 != null || assertAction2 != null)
            throw new InvalidOperationException("Assert can only be called once.");

        assertAction1 = action;
        return this;
    }

    public DatabaseTest Assert(Action<DbContext, dynamic> action)
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
                DbContext dbContext = new(dbDirectoryPath);
                await arrangeAction1(dbContext, context);
                dbContext.Dispose();
            }

            if (arrangeAction2 != null)
            {
                DbContext dbContext = new(dbDirectoryPath);
                arrangeAction2(dbContext, context);
                dbContext.Dispose();
            }

            if (actAction1 != null)
            {
                DbContext dbContext = new(dbDirectoryPath);
                await actAction1(dbContext, context);
                dbContext.Dispose();
            }

            if (actAction2 != null)
            {
                DbContext dbContext = new(dbDirectoryPath);
                actAction2(dbContext, context);
                dbContext.Dispose();
            }

            if (assertAction1 != null)
            {
                DbContext dbContext = new(dbDirectoryPath);
                await assertAction1(dbContext, context);
            }

            if (assertAction2 != null)
            {
                DbContext dbContext = new(dbDirectoryPath);
                assertAction2(dbContext, context);
            }
        }
        finally
        {
            RemoveDatabase();
        }
    }

    private void RemoveDatabase()
    {
        if (Directory.Exists(dbDirectoryPath))
            Directory.Delete(dbDirectoryPath, true);
    }
}