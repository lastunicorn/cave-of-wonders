# EF Core Migrations (SQLite Adapter)

## 1. How this interacts with the compiled model

They are independent artifacts that both need to stay in sync with the same source of truth (`Entities/` + `Configurations/`), but neither is derived from the other:

- `dotnet ef migrations add` / `dotnet ef database update` always build the model via the **real, reflection-based pipeline** (through `CaveOfWondersDbContextFactory`, which never calls `.UseModel(...)`). They never read `CompiledModels/`.
- `Database.Migrate()` at runtime applies migrations by running each migration's imperative `Up()` method (plain `migrationBuilder.CreateTable(...)` calls) — it does not consult `DbContext.Model` at all. So applying migrations works identically whether or not a compiled model is wired up.
- The compiled model (`CompiledModels/`) is only consulted for **querying and change-tracking** once the app is running — translating LINQ to SQL, tracking entity state, etc. It has no say in what tables/columns actually exist.

Net effect: adding migrations didn't require any change to how the compiled model is generated or wired, and vice versa. But it does mean **two** generated-code regeneration steps instead of one whenever the schema changes (§6.3).

## 2. Maintenance checklist when changing the schema

For any change to an entity class or its `IEntityTypeConfiguration<T>`, do all of these, in order, and commit them together:

1. Change the entity/configuration class (and `OnModelCreating` if it's a new entity).

2. ```bash
   cd sources/CaveOfWonders.Adapters.DataAccess.SQLite
   dotnet ef migrations add <DescriptiveName> --output-dir Migrations --configuration Release
   ```

   Review the generated `Up()`/`Down()` — EF's diff-based migration generation can occasionally pick a destructive strategy (e.g. drop-and-recreate a column) where a targeted `AlterColumn` would do; SQLite's limited `ALTER TABLE` support makes EF fall back to a rebuild-the-table strategy more often than other providers, so double-check any migration touching an existing (non-nullable/keyed) column.

3. Regenerate the compiled model (§4's regeneration steps) — same trigger conditions, now happening alongside every migration rather than instead of one.

4. Rebuild, run the integration suite (`dotnet test sources/CaveOfWonders.Tests.Integration`), and smoke-test the CLI against a scratch database.

5. Commit the migration files and the compiled-model files together with the entity/configuration change.

If you ever need to inspect what's actually applied against a given database file, query it directly:

```sql
SELECT MigrationId FROM __EFMigrationsHistory;
```