using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace DustInTheWind.CaveOfWonders.DbMigration.Logging;

/// <summary>
/// Writes only the log message (and exception, if any), without the default
/// "info: Category[EventId]" prefix, so the tool's output stays clean CLI text.
/// </summary>
internal sealed class PlainConsoleFormatter : ConsoleFormatter
{
    public const string FormatterName = "plain";

    public PlainConsoleFormatter()
        : base(FormatterName)
    {
    }

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
    {
        string message = logEntry.Formatter(logEntry.State, logEntry.Exception);

        if (message is null && logEntry.Exception is null)
            return;

        textWriter.WriteLine(message);

        if (logEntry.Exception is not null)
            textWriter.WriteLine(logEntry.Exception.ToString());
    }
}
