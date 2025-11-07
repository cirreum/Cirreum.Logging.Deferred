namespace Cirreum.Logging.Deferred.Internal;
/// <summary>
/// Maintains the global state for deferred logging.
/// </summary>
internal static class DeferredLogState {
	/// <summary>
	/// Stores the current logging scopes for each async context.
	/// </summary>
	internal static readonly AsyncLocal<Stack<IDisposable>> Scopes = new();

	/// <summary>
	/// Stores the queue of log entries waiting to be flushed.
	/// </summary>
	internal static readonly Queue<LogEntry> LogQueue = new();
}