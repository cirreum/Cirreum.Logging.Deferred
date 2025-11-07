namespace Cirreum.Logging.Deferred.Internal;

using Microsoft.Extensions.Logging;

/// <summary>
/// Represents a queued log entry with its associated metadata.
/// </summary>
/// <param name="Level">The severity level of the log message.</param>
/// <param name="Message">The message template of the log entry.</param>
/// <param name="Args">The arguments to be formatted into the message template.</param>
/// <param name="Scopes">The logging scopes active when this entry was created.</param>
internal record LogEntry(LogLevel Level, string Message, object[] Args, IReadOnlyList<object> Scopes);