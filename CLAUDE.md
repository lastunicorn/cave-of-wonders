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
Domain / DataTypes          — pure entities and enums, no dependencies
Infrastructure              — shared utilities (AsyncEnumerableExtensions, EnumerableExtensions, etc.)
Cli.Application             — use cases (MediatR request/handler pairs), depend on Ports
Ports.*                     — interfaces for external concerns (data access, BNR, INS, spreadsheets, files, clock, log)
Adapters.*                  — concrete implementations of Port interfaces
Cli / WebApi                — entry points that wire everything together (Autofac for CLI, MS DI for WebApi)
Cli.Presentation            — ConsoleTools.Commando commands + views (CLI only)
WebApi.Presentation         — ASP.NET Core controllers and DTOs
```

**Key conventions:**
- Every use case lives in its own folder under `Cli.Application/` with `*Request`, `*Response`, and `*UseCase` files. The use case class is a MediatR `IRequestHandler`.
- Port interfaces are pure `I*` contracts in `Ports.*` projects; adapters implement them in `Adapters.*` projects.
- The JSON data adapter (`Adapters.DataAccess.Json`) is the primary storage: it stores data as JSON files in a directory path configured via `ConnectionStrings:DefaultConnection` in `appsettings.json`. The LiteDb adapter (`Adapters.DataAccess.LiteDb`) only handles pots and exchange rates (no gems).
- JSON storage model types are prefixed with `J` (e.g., `JGem`, `JPot`, `JExchangeRate`) and live in `*Storage/` subdirectories.
- CLI commands use **ConsoleTools.Commando** for routing. Each command class in `Cli.Presentation` is auto-discovered via `RegisterCommandsFrom`. Commands are organized into area folders: `PotArea`, `FxArea`, `CpiArea`, `WageArea`, `WealthArea`.
- The Web API uses ASP.NET Core `[ApiController]` controllers (not minimal APIs) in `WebApi.Presentation/Endpoints/`. It uses **ErrorFlow** (`DustInTheWind.ErrorFlow.AspNetCore`) for structured exception-to-HTTP-response mapping, with one handler class per exception type in `WebApi.Presentation/ErrorHandlers/`.
- The `Infrastructure` project provides custom `IAsyncEnumerable<T>` extension methods used throughout the solution instead of `System.Linq.Async`: `ToListAsync`, `Select`, `FirstAsync`, `SingleAsync`, `SingleOrDefaultAsync`.

## JSON storage model

`Database.cs` in `Adapters.DataAccess.Json` is an in-memory working set. It loads data from disk on initialization and flushes via `IUnitOfWork.SaveChangesAsync()`. Use cases access all repositories through `IUnitOfWork`, which is the only constructor-injected data-access dependency they should take. Gems are lazy-loaded on first access (`LoadGemsAsync` is guarded by an `areGemsLoaded` flag); all other collections load eagerly on `Database.LoadAsync`.

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
- `CaveOfWonders.Tests.Integration` — integration tests for the JSON and LiteDb adapters. Uses a `DatabaseTest<TDb>` fluent builder where each phase (`Arrange` → `Act` → `Assert`) opens and closes a real database instance independently, ensuring persistence is exercised between phases. Call `Execute()` to run. The temporary database directory is deleted in `finally`.
- `tests/` (root) — Excel scenario analysis files (not automated tests).

## Namespace root

All namespaces are rooted at `DustInTheWind.CaveOfWonders.*`. The CLI binary assembly name is `cave`. The base exception class for use-case errors is `CaveOfWandersException` in `Cli.Application` (note: intentional "Wanders" spelling, matching the Mintos adapter namespace `DustInTheWind.CaveOfWanders.*` — do not correct either).
