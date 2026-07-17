# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working inside `CaveOfWonders.Cli.Application`.

## What lives here

One use case per folder. A use case is a single MediatR request/handler pair that implements one user-facing action. Each folder is self-contained: the request, response, use case class, any DTOs, and any use-case-specific exceptions all live inside it together.

Shared types that are not specific to a single use case (e.g. `CurrencyValue`, `CaveOfWandersException`, `StorageInaccessibleException`) live at the project root, not inside a use case folder.

## Folder and class naming

Folder names are PascalCase action phrases, verb first: `CalculateGain`, `CreatePot`, `ImportGems`, `PresentPots`, `ExportInflation`. The classes inside follow the same base name but may abbreviate the verb when unambiguous:

| File | Class name pattern | Example |
|------|--------------------|---------|
| Request | `<Name>Request` | `GainRequest`, `ImportGemsRequest` |
| Response | `<Name>Response` | `GainResponse`, `ImportGemsResponse` |
| Use case | `<Name>UseCase` | `GainUseCase`, `ImportGemsUseCase` |
| DTOs / items | descriptive noun | `GainItem`, `CpiDto`, `PotDetails`, `PotSummary` |
| Exceptions | `<Condition>Exception` | `PotNameNotSpecifiedException`, `StorageInaccessibleException` |

The class name prefix does not have to repeat the full folder name — use the shortest unambiguous form (folder `CalculateGain` → classes `Gain*`).

## Access modifiers

- **Request, Response, DTO classes** — `public`. They cross the boundary into the presentation layer.
- **Use case class** — `internal`. MediatR discovers it by reflection; the presentation layer never references it directly.
- **DTO constructors that accept a domain object** — `internal`. The use case constructs the DTO; the presentation layer only reads its properties.
- **Helper classes used only within one use case** — `internal`. If a use case becomes complex, extract helpers into the same folder (or an `<FolderName>/Helpers/` subfolder). Do not make them `public`.

## Properties

Request properties:
```csharp
public string Name { get; set; }
public DateOnly? StartDate { get; set; }
```

Response / DTO properties — use `init` for immutable data objects, `set` when the use case builds the object incrementally:
```csharp
// Immutable item built in a single expression:
public string PotName { get; init; }

// Response populated step-by-step inside the use case:
public int AddedCount { get; set; }
public int SkippedCount { get; set; }
```

Collection properties on responses are initialised at the declaration site:
```csharp
public List<GainItem> Items { get; init; } = [];
```

## Use case structure

```csharp
internal class ExampleUseCase : IRequestHandler<ExampleRequest, ExampleResponse>
{
    private readonly IUnitOfWork unitOfWork;
    // Add IClock only when the current date/time is needed as a default.

    public ExampleUseCase(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ExampleResponse> Handle(ExampleRequest request, CancellationToken cancellationToken)
    {
        // ...
    }
}
```

Rules:
- `IUnitOfWork` is the only data-access dependency. Never inject individual repositories (`IGemRepository`, `IPotRepository`, etc.) directly.
- `IClock` (from `Ports.ClockAccess`) is the only acceptable source of the current date or time.
- Other port interfaces (`IBnrService`, `IMintosService`, `IFintownService`, etc.) may be injected when the use case explicitly orchestrates an external system.
- All constructor parameters are null-checked with `?? throw new ArgumentNullException(nameof(...))`.

## Async and IAsyncEnumerable

Use the standard extension methods from the `System.Linq.AsyncEnumerable` NuGet package (version `10.0.0`, already referenced in this project's csproj). `System.Linq` is auto-imported via `ImplicitUsings`, so no extra `using` directive is needed:

```csharp
// Materialise a full sequence:
List<Gem> gems = await unitOfWork.GemRepository.FindAsync(filter, cancellationToken).ToListAsync();

// Take the single expected element (throws if 0 or >1):
Pot pot = await unitOfWork.PotRepository.GetByIdOrNameAsync(potFlexId, cancellationToken).SingleAsync();

// Optional single element:
Gem existing = await unitOfWork.GemRepository.GetByExternalIdAsync(...).SingleOrDefaultAsync();
```

Available extensions: `ToListAsync`, `Select`, `FirstAsync`, `FirstOrDefaultAsync`, `SingleAsync`, `SingleOrDefaultAsync`, and many more — same names as standard LINQ but on `IAsyncEnumerable<T>`.

## Gem queries

Prefer `FindAsync(GemFilter, cancellationToken)` over adding new single-purpose methods to `IGemRepository`. Build a `GemFilter` with only the predicates you need — unused properties are ignored:

```csharp
List<Gem> gains = await unitOfWork.GemRepository
    .FindAsync(new GemFilter
    {
        Month = month,
        Categories = [GemCategory.Gain, GemCategory.Loss]
    }, cancellationToken)
    .ToListAsync();
```

`GemFilter` supports: `PotId`, `Date`, `Month`, `Categories`, `ExternalId`.

## Mutations and saving

After any write operation, always call `SaveChangesAsync`:

```csharp
unitOfWork.PotRepository.Add(pot);
await unitOfWork.SaveChangesAsync(cancellationToken);
```

Wrap storage calls that can fail with infrastructure exceptions inside a `try/catch` and rethrow as `StorageInaccessibleException`:

```csharp
try
{
    unitOfWork.PotRepository.Add(pot);
    await unitOfWork.SaveChangesAsync(cancellationToken);
}
catch (Exception ex)
{
    throw new StorageInaccessibleException(ex);
}
```

## Exceptions

Throw specific exceptions for domain validation failures — never throw `Exception` or `InvalidOperationException` directly:

```csharp
if (string.IsNullOrWhiteSpace(request.Name))
    throw new PotNameNotSpecifiedException();
```

Each use-case-specific exception lives in the same folder as the use case that throws it and inherits from `CaveOfWandersException` (note: intentional "Wanders" spelling). `StorageInaccessibleException` and `CaveOfWandersException` are shared and live at the project root.

```csharp
public class PotNameNotSpecifiedException : CaveOfWandersException
{
    public PotNameNotSpecifiedException()
        : base("The pot name was not specified.")
    {
    }
}
```

## DTO construction patterns

Two established patterns — pick by context:

**Constructor-based** (older, richer mapping):
```csharp
public class PotDetails
{
    public Guid Id { get; }
    public string Name { get; }

    internal PotDetails(Pot pot)  // internal: only the use case constructs this
    {
        Id = pot.Id;
        Name = pot.Name;
    }
}
```

**Init-based** (newer, simpler data carriers):
```csharp
public class GainItem
{
    public string PotName { get; init; }
    public string Currency { get; init; }
    public decimal TotalGain { get; init; }
}
```

Use the constructor pattern when mapping from a domain object involves non-trivial logic. Use init-properties when the use case assembles the DTO directly with a LINQ projection.

## Private method naming

Extract private helpers with intent-revealing names. Use noun phrases for retrieval, verb phrases for actions:

```csharp
private MonthDate DecideMonth(GainRequest request) { ... }
private async Task<IEnumerable<Pot>> RetrievePots(...) { ... }
private async Task<IEnumerable<BnrExchangeRate>> ImportFromWebNbrFile(...) { ... }
```

## File header

Every `.cs` file carries the GPL licence comment block at the top (copy from any existing file in the project).
