// Cave of Wonders
// Copyright (C) 2023-2024 Dust in the Wind
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

using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures;

internal sealed class TempSqliteDatabase : IDisposable, IAsyncDisposable
{
	private readonly string dbDirectoryPath = Path.Combine(Path.GetTempPath(), $"test-database-{Guid.NewGuid()}");
	private readonly string connectionString;
	
	private CaveOfWondersDbContext dbContext;
	
	public TempSqliteDatabase()
	{
		string dbFilePath = Path.Combine(dbDirectoryPath, "test-database.db");
		connectionString = $"Data Source={dbFilePath}";
	}

	public async Task OpenAsync(CancellationToken cancellationToken = default)
	{
		Directory.CreateDirectory(dbDirectoryPath);

		DbContextOptions<CaveOfWondersDbContext> options = new DbContextOptionsBuilder<CaveOfWondersDbContext>()
			.UseSqlite(connectionString)
			.Options;

		dbContext = new CaveOfWondersDbContext(options);
		await dbContext.Database.EnsureCreatedAsync(cancellationToken);
	}

	public async Task CloseAsync(CancellationToken cancellationToken = default)
	{
		await dbContext.SaveChangesAsync(cancellationToken);

		await dbContext.DisposeAsync();
		dbContext = null;
	}
	
	public void Dispose()
	{
		dbContext?.Dispose();
		dbContext = null;

		DeleteDatabaseDirectory();
	}

	private void DeleteDatabaseDirectory()
	{
		// A pooled connection can keep the .db/-wal/-shm files open behind the scenes even after
		// Dispose, which would otherwise make the directory deletion below fail intermittently.
		SqliteConnection sqliteConnection = new(connectionString);
		SqliteConnection.ClearPool(sqliteConnection);

		if (Directory.Exists(dbDirectoryPath))
			Directory.Delete(dbDirectoryPath, true);
	}

	public async ValueTask DisposeAsync()
	{
		if (dbContext != null)
		{
			await dbContext.DisposeAsync();
			dbContext = null;
		}
		
		DeleteDatabaseDirectory();
	}
}