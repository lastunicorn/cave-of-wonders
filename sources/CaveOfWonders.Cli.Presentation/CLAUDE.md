# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working inside `CaveOfWonders.Cli.Presentation`.

## What lives here

One command per folder. Each folder contains exactly three types of files: the command class, the view, and the view model. These folders are grouped inside area folders that reflect the domain concept.

Shared display infrastructure (reusable `DataGrid` helpers, `Controls/`) lives at the project root, not inside command folders.

## Directory layout

```
<Area>/                  e.g. PotArea/, FxArea/, CpiArea/, WageArea/
    <CommandFolder>/     e.g. Pot/, GemImport/, FxImport/, Gain/, Wealth/
        *Command.cs
        *View.cs
        *ViewModel.cs
        [local enums or sub-view-models if needed]
Controls/                reusable ConsoleTools controls
```

Area names: use the domain noun with the `Area` suffix (`PotArea`, `FxArea`, `CpiArea`, `WageArea`). There is no separate `WealthArea` — `Gain` and `Wealth` (the former "wealth" commands) live under `PotArea`.

Command folder names: PascalCase noun phrase describing what the command does — not derived from the CLI verb name. Query commands use a noun alone (`Pot`, `Gain`); action commands use verb + noun (`GemImport`, `FxImport`, `PotCreate`).

## Command class

```csharp
[NamedCommand("my-command", Description = "One-line description shown in help.")]
internal class MyCommand : IConsoleCommand<MyViewModel>
{
    private readonly IMediator mediator;

    // --- Parameters ---

    [AnonymousParameter(DisplayName = "Pot Identifier", Order = 1, IsMandatory = false,
        Description = "Name or id of the pot. Partial id is accepted.")]
    public string PotIdentifier { get; set; }

    [NamedParameter("month", ShortName = 'm', IsMandatory = false,
        Description = "The month. Default = current month.")]
    public string Month { get; set; }

    [NamedParameter("all", ShortName = 'a', IsMandatory = false,
        Description = "Include inactive pots.")]
    public bool IncludeInactive { get; set; }

    // --- Constructor ---

    public MyCommand(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    // --- Execute ---

    public async Task<MyViewModel> Execute()
    {
        MyRequest request = new()
        {
            PotFlexId = PotIdentifier,
            Month = Month
        };

        MyResponse response = await mediator.Send(request);

        return new MyViewModel
        {
            Items = response.Items
        };
    }
}
```

Rules:
- Access modifier: `internal`. (Commando discovers commands by reflection; the bootstrapper only needs the `PresentationAssemblyHandle` marker class to locate the assembly.)
- Only dependency: `IMediator`. Null-check it in the constructor. Never inject anything else.
- `Execute()` does exactly three things: build the request, call `mediator.Send`, return a mapped view model. No branching, no error handling, no business logic.

## Command name (CLI verb)

The `[NamedCommand]` verb is the string users type at the prompt. Use:
- A single noun for read-only queries: `pot`, `fx`, `gain`, `wealth`, `wage`, `cpi`
- Kebab-case `noun-verb` for actions: `gem-import`, `fx-import`, `fx-convert`, `cpi-import`, `pot-create`, `pot-import`, `wage-import`

## Parameter attributes

### Named parameters  `[NamedParameter]`

```csharp
[NamedParameter("date", ShortName = 'd', IsMandatory = false,
    Description = "The date for which to display data.")]
public DateOnly? Date { get; set; }
```

- Long name: kebab-case (`source-type`, `start-date`, `file-type`)
- `ShortName`: a single character, typically the first letter of the long name
- Always set `IsMandatory` explicitly
- Add a `Description` on every parameter; it appears in auto-generated help
- Default values can be set at the property declaration: `public CpiImportSourceType ImportSourceType { get; set; } = CpiImportSourceType.Web;`

### Positional parameters  `[AnonymousParameter]`

```csharp
[AnonymousParameter(DisplayName = "Pot Identifier", Order = 1, IsMandatory = false,
    Description = "Name or id of the pot. Partial id is accepted.")]
public string PotIdentifier { get; set; }
```

- `Order` starts at `1`
- Use for the most natural "primary argument" of a command (e.g., the identifier of the thing being queried)

### Supported parameter types

Commando parses the following types directly:

| C# type | Example |
|---------|---------|
| `string` | any text |
| `bool` | flag parameter; presence = `true` |
| `bool?` | optional flag |
| `int`, `int?` | integer |
| `uint`, `uint?` | non-negative integer |
| `DateOnly`, `DateOnly?` | date string |
| `CultureInfo` | culture identifier |
| custom `enum` | parsed by name (case-insensitive) |

When a parameter accepts a fixed set of string values, define a local `internal enum` in the same command folder. Map it to the application layer's enum inside `Execute()`:

```csharp
// CpiImport/CpiImportSourceType.cs
internal enum CpiImportSourceType { File, Web }

// CpiImport/CpiImportCommand.cs
ImportSource = ImportSourceType switch
{
    CpiImportSourceType.File => ImportSource.File,
    CpiImportSourceType.Web  => ImportSource.Web,
    _ => throw new ArgumentOutOfRangeException()
},
```

## View class

The view renders a view model to the console. Commando matches a view to its command by the view model type parameter.

**Simple view** — implement `IView<TViewModel>`:

```csharp
internal class GainView : IView<GainViewModel>
{
    public void Display(GainViewModel viewModel)
    {
        DataGrid dataGrid = new();
        dataGrid.Columns.Add("Pot");
        dataGrid.Columns.Add(new Column("Total Gain")
        {
            CellHorizontalAlignment = HorizontalAlignment.Right
        });

        foreach (GainItem item in viewModel.Items)
            dataGrid.Rows.Add(item.PotName, item.TotalGain);

        dataGrid.Display();
    }
}
```

**Complex view** — extend `ViewBase<TViewModel>` when the display has multiple conditional rendering paths:

```csharp
internal class PotView : ViewBase<PotCommandViewModel>
{
    public override void Display(PotCommandViewModel viewModel)
    {
        if (viewModel.PotDetails?.Count > 0)
            DisplayDetails(viewModel.PotDetails);
        else
            DisplaySummary(viewModel);
    }

    private static void DisplayDetails(...) { ... }
    private static void DisplaySummary(...) { ... }
}
```

Rules:
- Access modifier: `internal`
- No constructor parameters — views have no injected dependencies
- `Display` should only render; it never mutates state or calls use cases

### Rendering helpers

Use `DataGridTemplate.CreateNew()` (defined at project root) instead of `new DataGrid()` when the output needs the standard styled title row with grey background:

```csharp
DataGrid dataGrid = DataGridTemplate.CreateNew();
dataGrid.Title = "Pots";
```

Use `CustomConsole` (from `DustInTheWind.ConsoleTools`) for simple non-tabular output:

```csharp
CustomConsole.WriteLineSuccess("Import succeeded.");
CustomConsole.WriteLineWarning("No pot found with that name.");
CustomConsole.WriteLine($"  Added: {viewModel.AddedCount}");
```

For cell-level styling: set `ForegroundColor` on `Column` or directly on `ContentRow[columnIndex]`.

## ViewModel class

The view model is a display-only data container. Commando uses the type parameter on `IConsoleCommand<T>` to locate the matching view.

**Flat view model** — when the command maps the response inline in `Execute()`:

```csharp
internal class GainViewModel
{
    public List<GainItem> Items { get; set; }
}
```

Properties: `{ get; set; }` or `{ get; init; }` — whichever reads more naturally.

**Constructor view model** — when mapping from the response is non-trivial or the view model wraps complex structures:

```csharp
internal class FxImportViewModel
{
    public int AddedCount { get; }
    public List<UpdateValueViewModel> Updates { get; }

    internal FxImportViewModel(ImportExchangeRatesResponse report)
    {
        AddedCount = report.AddedCount;
        Updates = report.Updates.Select(x => new UpdateValueViewModel(x)).ToList();
    }
}
```

Properties: `{ get; }` (readonly, set in constructor). Constructor access modifier: `internal` when it takes application-layer types.

**Sub-view models** — when the view model contains other view model types (e.g., one row per item), define them in the same folder:

```csharp
// PotArea/Pot/PotDetailsViewModel.cs
internal class PotDetailsViewModel
{
    public Guid Id { get; }
    public string Name { get; }

    internal PotDetailsViewModel(PotDetails potDetails) { ... }
}
```

**Skipping the view model entirely** — when the application `*Response` is simple and the view can consume it directly, the command can declare `IConsoleCommand<TResponse>` and return the response unchanged from `Execute()`:

```csharp
public class FxCommand : IConsoleCommand<PresentExchangeRateResponse>
{
    public async Task<PresentExchangeRateResponse> Execute()
    {
        ...
        return await mediator.Send(request);
    }
}
```

The corresponding view then implements `IView<PresentExchangeRateResponse>`.

## Namespace

```
DustInTheWind.CaveOfWonders.Cli.Presentation.<Area>.<Folder>
```

Examples:
- `DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Gain`
- `DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.GemImport`
- `DustInTheWind.CaveOfWonders.Cli.Presentation` (shared utilities at root)

## Assembly discovery

`PresentationAssemblyHandle` (at the project root) is a public marker class referenced by the CLI bootstrapper so ConsoleTools.Commando can scan this assembly for commands and views. Do not delete it.

## File header

Every `.cs` file carries the GPL licence comment block at the top (copy from any existing file in the project).
