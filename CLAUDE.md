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
- The JSON data adapter (`Adapters.DataAccess.Json`) is the primary storage: it stores data as JSON files in a directory path configured via `Database:Path` in `appsettings.json`. There is also a LiteDb adapter (`Adapters.DataAccess.LiteDb`) for pot/exchange-rate storage.
- JSON storage model types are prefixed with `J` (e.g., `JGem`, `JPot`, `JExchangeRate`) and live in `*Storage/` subdirectories.
- CLI commands use **ConsoleTools.Commando** for routing. Each command class in `Cli.Presentation` is auto-discovered via `RegisterCommandsFrom`. Commands are organized into area folders: `PotArea`, `FxArea`, `CpiArea`, `WageArea`, `WealthArea`.
- The Web API uses **ErrorFlow** (`DustInTheWind.ErrorFlow.AspNetCore`) for structured exception-to-HTTP-response mapping. Error handlers in `WebApi.Presentation/ErrorHandlers/` each handle one specific exception type.

## Domain concepts

- **Pot** — a money account with a currency, display order, and a time range (StartDate / EndDate).
- **PotSnapshot** — a dated balance record for a pot.
- **Gem** — a transaction (deposit, withdrawal, gain, loss, bonus) associated with a pot, with a `GemCategory` enum.
- **ExchangeRate** — daily BNR currency rates, stored per currency-pair per day.
- **Cpi / AverageWage** — CPI (Consumer Price Index) and average wage records, imported from INS.

## Test projects

- `CaveOfWonders.Tests` — unit tests (xUnit, Moq, FluentAssertions). Tests use case logic against mocked port interfaces.
- `CaveOfWonders.IntegrationTests` — integration tests for the application layer.
- `tests/` (root) — Excel scenario analysis files (not automated tests).

## Namespace root

All namespaces are rooted at `DustInTheWind.CaveOfWonders.*`. The CLI binary assembly name is `cave`.
