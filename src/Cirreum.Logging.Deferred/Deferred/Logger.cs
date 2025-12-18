namespace Cirreum.Logging.Deferred;

using Cirreum.Logging.Deferred.Internal;
using Microsoft.Extensions.Logging;

/// <summary>
/// Provides factory methods for creating deferred loggers.
/// </summary>
public static partial class Logger {

	/// <summary>
	/// Creates a new deferred logger instances
	/// </summary>
	/// <returns>An <see cref="IDeferredLogger"/> that queues log messages for later processing.</returns>
	public static IDeferredLogger CreateDeferredLogger() => new DeferredLogQueue();

	/// <summary>
	/// Does the deferred log contain any errors.
	/// </summary>
	/// <returns></returns>
	public static bool HasErrors() {
		return DeferredLogState.LogQueue
			.Any(entry => entry.Level == LogLevel.Error);
	}

	/// <summary>
	/// Get all error entries.
	/// </summary>
	/// <returns></returns>
	public static IEnumerable<string> GetErrors() {
		return DeferredLogState.LogQueue
			.Where(entry => entry.Level == LogLevel.Error)
			.Select(entry => FormatMessage(entry.Message, entry.Args));
	}

	/// <summary>
	/// Does the deferred log contain any entries with the specified log level.
	/// </summary>
	/// <param name="level">The level to evaluate.</param>
	/// <returns></returns>
	public static bool HasEntries(LogLevel level) {
		return DeferredLogState.LogQueue
			.Any(entry => entry.Level == level);
	}

	/// <summary>
	/// Get all entries for the specified level.
	/// </summary>
	/// <returns></returns>
	public static IEnumerable<(LogLevel Level, string Message)> GetAll(LogLevel level) {
		return DeferredLogState.LogQueue
			.Where(entry => entry.Level == level)
			.Select(entry => (entry.Level, FormatMessage(entry.Message, entry.Args)));
	}

	/// <summary>
	/// Get all entries.
	/// </summary>
	/// <returns></returns>
	public static IEnumerable<(LogLevel Level, string Message)> GetAll() {
		return DeferredLogState.LogQueue
			.Select(entry => (entry.Level, FormatMessage(entry.Message, entry.Args)));
	}

	private static string FormatMessage(string template, object[] args) {
		if (args.Length == 0) {
			return template;
		}

		// Convert structured logging placeholders {Name} to positional {0}, {1}, etc.
		var index = 0;
		var formatted = StructuredPositionRegEx().Replace(template, _ => $"{{{index++}}}"
);

		try {
			return string.Format(formatted, args);
		} catch (FormatException) {
			return template;
		}
	}

	[System.Text.RegularExpressions.GeneratedRegex(@"\{[^}]+\}")]
	private static partial System.Text.RegularExpressions.Regex StructuredPositionRegEx();

}