namespace Cirreum.Logging.Deferred;

/// <summary>
/// Defines a logger that queues log messages for later processing.
/// </summary>
/// <remarks>
/// Use this interface during application startup or configuration phases
/// where the logging infrastructure isn't yet available.
/// </remarks>
public interface IDeferredLogger {

	/// <summary>
	/// Creates a new scope for logging.
	/// </summary>
	/// <typeparam name="TState">The type of the state to create the scope with.</typeparam>
	/// <param name="state">The state to create the scope with.</param>
	/// <returns>An <see cref="IDisposable"/> that ends the scope when disposed.</returns>
	IDisposable BeginScope<TState>(TState state) where TState : notnull;

	/// <summary>
	/// Queues a debug log message.
	/// </summary>
	void LogDebug(string message, params object[] args);

	/// <summary>
	/// Queues an information log message.
	/// </summary>
	void LogInformation(string message, params object[] args);

	/// <summary>
	/// Queues a warning log message.
	/// </summary>
	void LogWarning(string message, params object[] args);

	/// <summary>
	/// Queues an error log message.
	/// </summary>
	void LogError(string message, params object[] args);

	/// <summary>
	/// Queues a critical log message.
	/// </summary>
	void LogCritical(string message, params object[] args);

	/// <summary>
	/// Queues a trace log message.
	/// </summary>
	void LogTrace(string message, params object[] args);

}