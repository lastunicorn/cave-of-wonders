using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;

namespace DustInTheWind.CaveOfWonders.IntegrationTests.Adapters.DataAccess.Json.Infrastructure;

internal class DatabaseTest
{
    private readonly string dbDirectoryPath;

    private Func<Database, dynamic, Task> arrangeAction1;
    private Action<Database, dynamic> arrangeAction2;

    private Func<Database, dynamic, Task> actAction1;
    private Action<Database, dynamic> actAction2;

    private Func<Database, dynamic, Task> assertAction1;
    private Action<Database, dynamic> assertAction2;

    public static DatabaseTest Create()
    {
        return new DatabaseTest();
    }

    private DatabaseTest()
    {
        dbDirectoryPath = Path.Combine(Path.GetTempPath(), $"test-database-{Guid.NewGuid()}");
    }

    public DatabaseTest Arrange(Func<Database, dynamic, Task> action)
    {
        if (arrangeAction1 != null || arrangeAction2 != null)
            throw new InvalidOperationException("Arrange can only be called once.");

        arrangeAction1 = action;
        return this;
    }

    public DatabaseTest Arrange(Action<Database, dynamic> action)
    {
        if (arrangeAction1 != null || arrangeAction2 != null)
            throw new InvalidOperationException("Arrange can only be called once.");

        arrangeAction2 = action;
        return this;
    }

    public DatabaseTest Act(Func<Database, dynamic, Task> action)
    {
        if (actAction1 != null || actAction2 != null)
            throw new InvalidOperationException("Act can only be called once.");

        actAction1 = action;
        return this;
    }

    public DatabaseTest Act(Action<Database, dynamic> action)
    {
        if (actAction1 != null || actAction2 != null)
            throw new InvalidOperationException("Act can only be called once.");

        actAction2 = action;
        return this;
    }

    public DatabaseTest Assert(Func<Database, dynamic, Task> action)
    {
        if (assertAction1 != null || assertAction2 != null)
            throw new InvalidOperationException("Assert can only be called once.");

        assertAction1 = action;
        return this;
    }

    public DatabaseTest Assert(Action<Database, dynamic> action)
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
                Database database = await OpenDatabase();
                await arrangeAction1(database, context);
                await database.SaveAsync(CancellationToken.None);
            }

            if (arrangeAction2 != null)
            {
                Database database = await OpenDatabase();
                arrangeAction2(database, context);
                await database.SaveAsync(CancellationToken.None);
            }

            if (actAction1 != null)
            {
                Database database = await OpenDatabase();
                await actAction1(database, context);
                await database.SaveAsync(CancellationToken.None);
            }

            if (actAction2 != null)
            {
                Database database = await OpenDatabase();
                actAction2(database, context);
                await database.SaveAsync(CancellationToken.None);
            }

            if (assertAction1 != null)
            {
                Database database = await OpenDatabase();
                await assertAction1(database, context);
            }

            if (assertAction2 != null)
            {
                Database database = await OpenDatabase();
                assertAction2(database, context);
            }
        }
        finally
        {
            RemoveDatabase();
        }
    }

    private async Task<Database> OpenDatabase()
    {
        Database database = new(dbDirectoryPath);
        await database.LoadAsync(CancellationToken.None);

        return database;
    }

    private void RemoveDatabase()
    {
        if (Directory.Exists(dbDirectoryPath))
            Directory.Delete(dbDirectoryPath, true);
    }
}