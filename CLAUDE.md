# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

Cave of Wonders is a personal finance tracking tool built in C# (.NET 8). It manages money accounts ("pots"), tracks snapshots of their balances over time, stores transaction records ("gems"), and maintains exchange rates (from BNR — National Bank of Romania) and CPI/inflation data (from INS — National Institute of Statistics). It ships as both a CLI (`cave`) and a Web API.

## Commands

```bash
# Restore, build, and test the entire solution
dotnet restore CaveOfWonders.slnx
dotnet build --configuration Release CaveOfWonders.slnx
dotnet test CaveOfWonders.slnx

# Run a single test project
dotnet test sources/CaveOfWonders.Tests/CaveOfWonders.Tests.csproj
dotnet test sources/CaveOfWonders.Tests.Integration/CaveOfWonders.Tests.Integration.csproj

# Run a specific test class or method
dotnet test sources/CaveOfWonders.Tests/CaveOfWonders.Tests.csproj --filter "FullyQualifiedName~ConvertUseCase"

# Run the CLI
dotnet run --project sources/CaveOfWonders.Cli

# Run the Web API
dotnet run --project sources/CaveOfWonders.WebApi
```

## Architecture

The codebase follows a **Ports & Adapters (Hexagonal) architecture** with a strict layering convention:

```
DataTypes                   — pure value types/primitives, no dependencies (Currency, CurrencyPair, GemCategory, PotFlexId)
Domain                      — entities, depend only on DataTypes (Pot, PotSnapshot, Gem, ExchangeRate, Cpi, AverageWage)
Infrastructure              — shared utilities (AsyncEnumerableExtensions, EnumerableExtensions, etc.)
Cli.Application             — use cases (MediatR request/handler pairs), depend on Ports
Ports.*                     — interfaces for external concerns (data access, BNR, INS, spreadsheets, files, clock, log, Mintos, Fintown)
Adapters.*                  — concrete implementations of Port interfaces
Cli / WebApi                — entry points that wire everything together (Autofac for CLI, MS DI for WebApi)
Cli.Presentation            — ConsoleTools.Commando commands + views (CLI only)
WebApi.Presentation         — ASP.NET Core controllers and DTOs
```

**Key conventions:**
- Every use case lives in its own folder under `Cli.Application/` with `*Request`, `*Response`, and `*UseCase` files. The use case class is a MediatR `IRequestHandler`.
- Port interfaces are pure `I*` contracts in `Ports.*` projects; adapters implement them in `Adapters.*` projects. `Ports.MintosAccess`/`Adapters.MintosAccess` and `Ports.FintownAccess`/`Adapters.FintownAccess` wrap the Mintos P2P-lending and Fintown investment platforms, used as gem-import sources.
- There are three data-access adapters, but **only the JSON adapter is wired into production** (CLI `Setup.cs` and WebApi `Program.cs` both register `Database`/`UnitOfWork` from `DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json`). The other two are exercised only by the integration test suite:
  - **`Adapters.DataAccess.Json`** (project folder is `sources/CaveOfWonders.Adapters.DataAccess/`, but the `.csproj`/assembly/namespace are still `...DataAccess.Json` — don't be misled by the folder name) — stores data as JSON files in a directory configured via `ConnectionStrings:DefaultConnection` in `appsettings.json`. Full support for all repositories, including gems.
  - **`Adapters.DataAccess.SQLite`** — EF Core-backed, also full support including gems (`CaveOfWondersDbContext`, `*Entity` classes).
  - **`Adapters.DataAccess.LiteDb`** — only implements `PotRepository` and `ExchangeRateRepository`; no gem/CPI/average-wage support.
  - `sources/CaveOfWonders.Tests.Integration/tests-config.json` lists which adapters each repository port is tested against (e.g. `IPotRepository` → Json/LiteDb/SQLite; `IGemRepository` → Json/SQLite only).
- JSON storage model types are prefixed with `J` (e.g., `JGem`, `JPot`, `JExchangeRate`) and live in `*Storage/` subdirectories.
- CLI commands use **ConsoleTools.Commando** for routing. Each command class in `Cli.Presentation` is auto-discovered via `RegisterCommandsFrom`. Commands are organized into area folders: `PotArea`, `FxArea`, `CpiArea`, `WageArea`, `WealthArea`.
- The Web API uses ASP.NET Core `[ApiController]` controllers (not minimal APIs) in `WebApi.Presentation/Endpoints/`. It uses **ErrorFlow** (`DustInTheWind.ErrorFlow.AspNetCore`) for structured exception-to-HTTP-response mapping, with one handler class per exception type in `WebApi.Presentation/ErrorHandlers/`.
- The `Infrastructure` project provides custom `IAsyncEnumerable<T>` extension methods used throughout the solution instead of `System.Linq.Async`: `ToListAsync`, `Select`, `FirstAsync`, `SingleAsync`, `SingleOrDefaultAsync`.

## JSON storage model

`Database.cs` in `Adapters.DataAccess.Json` (namespace `...DataAccess.Json`, folder `sources/CaveOfWonders.Adapters.DataAccess/`) is an in-memory working set. It loads data from disk on initialization and flushes via `IUnitOfWork.SaveChangesAsync()`. Use cases access all repositories through `IUnitOfWork`, which is the only constructor-injected data-access dependency they should take. Gems are lazy-loaded on first access (`LoadGemsAsync` is guarded by an `areGemsLoaded` flag); all other collections load eagerly on `Database.LoadAsync`.

## Gem querying

`IGemRepository` exposes two query styles:

- **Targeted methods** (`FindByMonthAsync(potId, month, ...)`, `GetByPotIdAsync(potId, ...)`, etc.) — use when the pot is already known.
- **`FindAsync(GemFilter, ...)`** — the preferred method for flexible cross-pot queries. `GemFilter` (in `Ports.DataAccess`) supports composable predicates: `PotId`, `Date`, `Month`, `Categories` (list — translated to `WHERE category IN (...)` in SQL adapters), and `ExternalId`. Prefer this over adding new single-purpose methods.

## Domain concepts

- **Pot** — a money account with a currency, display order, a time range (StartDate / EndDate), and a `Labels` list (`List<string>`) used for categorization (e.g. `"mintos"`, `"fintown"`).
- **PotSnapshot** — a dated balance record for a pot.
- **Gem** — a transaction (deposit, withdrawal, gain, loss, bonus) associated with a pot, with a `GemCategory` enum. Each `Gem` carries a populated `Pot` reference when returned from any repository method.
- **ExchangeRate** — daily BNR currency rates, stored per currency-pair per day.
- **Cpi / AverageWage** — CPI (Consumer Price Index) and average wage records, imported from INS.
- **`PotFlexId`** — a value type in `DataTypes` that matches a pot by either its GUID or its name string; used in repository lookups.
- **`MonthDate`** — a value type in `Infrastructure` with constructors `MonthDate(int year, int month)` and `MonthDate(DateOnly)`. Implicitly converts to/from `string` (`"MM/YYYY"` format). `HasValue` returns false for the default struct value.

## Test projects

- `CaveOfWonders.Tests` — unit tests (xUnit, Moq, FluentAssertions). Test use case logic against mocked port interfaces. Test method naming follows `HavingX_WhenY_ThenZ`.
- `CaveOfWonders.Tests.Utils` — shared integration-test infrastructure (see below).
- `CaveOfWonders.Tests.Integration` — integration tests for the data-access and other adapters, run once per adapter configured for that port in `tests-config.json`. Test classes live under `Ports/<PortArea>/<Interface>Tests/`, one file per method (e.g. `GemRepositoryTests/AddTests.cs`), each with an `ITestEnvironment<...>/TestEnvironments/` subfolder providing the per-adapter environment.
- `tests/` (root) — Excel scenario analysis files (not automated tests).

### Integration test pattern (Back Door Manipulation)

Each test is a `[Theory]` parameterized with `[TestEnvironments<TSut, TBackDoor>]`, which yields one `ITestEnvironment<TSut, TBackDoor>` per adapter listed for that port in `tests-config.json` — the test body runs once per adapter without change. `TSut` is the port interface under test (e.g. `IGemRepository`); `TBackDoor` (`ITestBackDoor`) is a separate, adapter-specific interface used to seed/inspect state directly, bypassing the SUT.

Use `GenericTest.Create(environment)` to build the test:
- `.Arrange(async (backDoor, context) => ...)` — seed data via the back door only.
- `.Act((repository, context) => ...)` — exercise the SUT only; the back door is inaccessible here by design.
- `.Assert(async (backDoor, context) => ...)` / `.AssertThrow(ex => ...)` — verify via the back door only.
- `context` is a `GenericTestContext` dynamic bag for passing values between phases.
- Call `.ExecuteAsync()` to run.

Giving each phase only `TSut` or only `TBackDoor` (never both) is what compiler-enforces the phase separation — this replaces the older `DatabaseTest<TDb>` pattern.

## Namespace root

All namespaces are rooted at `DustInTheWind.CaveOfWonders.*`. The CLI binary assembly name is `cave`. The base exception class for use-case errors is `CaveOfWandersException` in `Cli.Application` (note: intentional "Wanders" spelling, matching the Mintos adapter namespace `DustInTheWind.CaveOfWanders.*` — do not correct either).
