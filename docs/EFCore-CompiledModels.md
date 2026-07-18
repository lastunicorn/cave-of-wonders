# EF Core Compiled Models (SQLite Adapter)

## 1. Decision

`CaveOfWondersDbContext` uses an EF Core **compiled model** instead of letting EF Core build its model at runtime via reflection:

```csharp
options
    .UseSqlite(connectionString)
    .UseModel(CaveOfWondersDbContextModel.Instance);
```

`CaveOfWondersDbContextModel` is generated source code, checked into `sources/CaveOfWonders.Adapters.DataAccess.SQLite/CompiledModels/`, produced by the `dotnet-ef` CLI tool. It is **not** hand-written and should not be edited directly.

## 2. Why

The CLI is a short-lived process — a new `dotnet cave.dll ...` invocation per command, not a long-running server.

EF Core normally builds its model (scanning `DbSet<T>` properties and every `IEntityTypeConfiguration<T>` via reflection, then running it through the conventions pipeline) the first time the context is used — for a long-lived web app that cost is paid once per process lifetime, but a CLI process pays it on *every single invocation*.

Compiled models move that work to build time: `dotnet ef dbcontext optimize` runs the model-building pipeline once, ahead of time, and serializes the result as plain C# object construction — no reflection left at runtime.

## 3. How to maintain the compiled model

The generated files are a **frozen snapshot** of the model as of the last time `dotnet ef dbcontext optimize` was run. EF Core does **not** detect drift for you at runtime. The SQLite schema is created/updated by `dbContext.Database.Migrate()` applying migrations, but the compiled model is a *separate* snapshot used for querying/change-tracking — it does not come from the migrations and migrations do not come from it. If the compiled model goes stale, EF Core will build query/update SQL against the wrong shape (missing/renamed columns, wrong keys) even though the actual table structure (created by migrations) is current. This is a correctness footgun, not just a perf one — treat regeneration as mandatory, not optional cleanup.

### You MUST regenerate when you change any of:

- a `DbSet<T>` on `CaveOfWondersDbContext` (add/remove/rename)
- an entity class under `Adapters.DataAccess.SQLite/Entities/` (add/remove/rename a property, change a type)
- any `IEntityTypeConfiguration<T>` under `Adapters.DataAccess.SQLite/Configurations/` (column mapping, index, relationship, value conversion, etc.)
- add a brand-new entity + configuration pair (also add the explicit `modelBuilder.ApplyConfiguration(...)` line in `OnModelCreating`, and a new migration — see §6.3)

### Regeneration steps

1. Make sure the local tool is restored (one-time per clone, or after the manifest changes):
   ```bash
   dotnet tool restore
   ```
2. From the repo root, run:
   ```bash
   cd sources/CaveOfWonders.Adapters.DataAccess.SQLite
   dotnet ef dbcontext optimize \
     --output-dir CompiledModels \
     --namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.CompiledModels \
     --configuration Release
   ```
   (`dotnet tool run dotnet-ef -- dbcontext optimize ...` also works if `dotnet ef` isn't on PATH.)
3. Review the diff under `CompiledModels/` — it should only touch the entity/property/index you actually changed. A much larger diff than
   expected is a signal something else drifted.
4. If the change also altered the schema, see §4.2.

### Common mistakes

- **Editing a `CompiledModels/*.cs` file by hand.** It will be silently overwritten next time someone regenerates, and in the meantime it represents a model that doesn't match the C# source of truth (`Entities/`/`Configurations/`). Change the real entity/configuration instead and regenerate.
- **Forgetting to regenerate after a configuration change.** Nothing fails the build — the mismatch only shows up as wrong/missing columns or failed queries at runtime. If a change looks right in code but behaves oddly against SQLite, check whether `CompiledModels/` was regenerated and committed alongside it.
- **Running `dotnet ef` against a different EF Core version than the app
  uses.** The tool version is pinned in `.config/dotnet-tools.json` specifically to track `Microsoft.EntityFrameworkCore.Sqlite`'s version in `Adapters.DataAccess.SQLite.csproj` — bump both together if you upgrade EF Core.

