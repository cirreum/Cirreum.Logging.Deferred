namespace Cirreum.Logging.Deferred.Internal;

using Microsoft.Extensions.Logging;

internal sealed class DeferredLogQueue : IDeferredLogger {

	public IDisposable BeginScope<TState>(TState state) where TState : notnull {
		var scopes = DeferredLogState.Scopes.Value ??= new Stack<IDisposable>();
		var scope = new LogScope<TState>(state, scopes);
		scopes.Push(scope);
		return scope;
	}

	public void LogDebug(string message, params object[] args)
		=> QueueLog(LogLevel.Debug, message, args);

	public void LogInformation(string message, params object[] args)
		=> QueueLog(LogLevel.Information, message, args);

	public void LogWarning(string message, params object[] args)
		=> QueueLog(LogLevel.Warning, message, args);

	public void LogError(string message, params object[] args)
		=> QueueLog(LogLevel.Error, message, args);

	public void LogCritical(string message, params object[] args)
		=> QueueLog(LogLevel.Critical, message, args);

	public void LogTrace(string message, params object[] args)
		=> QueueLog(LogLevel.Trace, message, args);

	private static void QueueLog(LogLevel level, string message, params object[] args) {
		var currentScopes = DeferredLogState.Scopes.Value?
			.Select(s => ((ILogScope)s).GetState())
			.Reverse()
			.ToList() ?? [];

		DeferredLogState.LogQueue.Enqueue(new LogEntry(level, message, args, currentScopes));
	}

}