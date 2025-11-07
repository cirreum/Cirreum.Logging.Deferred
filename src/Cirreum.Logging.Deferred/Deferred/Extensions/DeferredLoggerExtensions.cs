namespace Cirreum.Logging.Deferred;

using Cirreum.Logging.Deferred.Internal;
using Microsoft.Extensions.Logging;

/// <summary>
/// Extension methods for flushing deferred logs to an <see cref="ILogger"/>.
/// </summary>
public static class DeferredLoggerExtensions {

	/// <summary>
	/// Flushes all queued deferred logs to the provided logger.
	/// </summary>
	/// <param name="logger">The logger to flush queued messages to.</param>
	/// <remarks>
	/// This method processes all queued log entries in order, maintaining their original log levels and scopes.
	/// After flushing, the queue is empty and subsequent calls will have no effect unless new logs have been queued.
	/// </remarks>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is null.</exception>
	public static void FlushDeferredLogs(this ILogger logger) {
		ArgumentNullException.ThrowIfNull(logger);

		while (DeferredLogState.LogQueue.TryDequeue(out var entry)) {
			using (var scope = entry.Scopes.Count > 0
				? logger.CreateNestedScope(entry.Scopes)
				: null) {
				logger.Log(entry.Level, entry.Message, entry.Args);
			}
		}

	}

	internal static CompositeDisposable CreateNestedScope(this ILogger logger, IReadOnlyList<object> scopes) {
		var disposables = new CompositeDisposable();
		foreach (var scope in scopes) {
			var disposable = logger.BeginScope(scope);
			if (disposable != null) {
				disposables.Add(disposable);
			}
		}
		return disposables;
	}

}