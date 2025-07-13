// Cave of Wonders
// Copyright (C) 2023-2025 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.


// Cave of Wonders
// Copyright (C) 2023-2025 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.


// Cave of Wonders
// Copyright (C) 2023-2025 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.


// Cave of Wonders
// Copyright (C) 2023-2025 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

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
                await database.Save();
            }

            if (arrangeAction2 != null)
            {
                Database database = await OpenDatabase();
                arrangeAction2(database, context);
                await database.Save();
            }

            if (actAction1 != null)
            {
                Database database = await OpenDatabase();
                await actAction1(database, context);
                await database.Save();
            }

            if (actAction2 != null)
            {
                Database database = await OpenDatabase();
                actAction2(database, context);
                await database.Save();
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
        await database.Load();

        return database;
    }

    private void RemoveDatabase()
    {
        if (Directory.Exists(dbDirectoryPath))
            Directory.Delete(dbDirectoryPath, true);
    }
}