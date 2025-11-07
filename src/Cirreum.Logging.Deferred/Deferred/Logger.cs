namespace Cirreum.Logging.Deferred;

using Cirreum.Logging.Deferred.Internal;
using Microsoft.Extensions.Logging;

/// <summary>
/// Provides factory methods for creating deferred loggers.
/// </summary>
public static class Logger {

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
			.Select(entry => entry.Message);
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
			.Select(entry => (entry.Level, entry.Message));
	}

	/// <summary>
	/// Get all entries.
	/// </summary>
	/// <returns></returns>
	public static IEnumerable<(LogLevel Level, string Message)> GetAll() {
		return DeferredLogState.LogQueue
			.Select(entry => (entry.Level, entry.Message));
	}

}