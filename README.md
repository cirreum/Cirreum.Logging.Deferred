# Cirreum.Logging.Deferred

[![NuGet Version](https://img.shields.io/nuget/v/Cirreum.Logging.Deferred.svg?style=flat-square)](https://www.nuget.org/packages/Cirreum.Logging.Deferred/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Cirreum.Logging.Deferred.svg?style=flat-square)](https://www.nuget.org/packages/Cirreum.Logging.Deferred/)
[![GitHub Release](https://img.shields.io/github/v/release/cirreum/Cirreum.Logging.Deferred?style=flat-square)](https://github.com/cirreum/Cirreum.Logging.Deferred/releases)

Provides deferred logging capabilities for early application setup and configuration phases where the logging infrastructure isn't yet available

## Overview

During application startup, you often need to log important information before the dependency injection container and logging infrastructure are fully configured. This library allows you to queue log messages early in the application lifecycle and flush them to the actual logger once it becomes available.

## Features

- **Thread-safe logging**: Queue log messages from any thread during startup
- **Scope preservation**: Maintains logging scopes across deferred operations  
- **All log levels**: Supports Debug, Information, Warning, Error, Critical, and Trace levels
- **Easy integration**: Simple extension method to flush queued logs to any `ILogger`
- **Startup task support**: Works seamlessly with startup task patterns

## Installation

```bash
dotnet add package Cirreum.Logging.Deferred
```

## Usage

### Basic Usage

```csharp
using Cirreum.Logging.Deferred;

// Create a deferred logger early in your application
var deferredLogger = Logger.CreateDeferredLogger();

// Queue log messages before logging infrastructure is ready
deferredLogger.LogInformation("Application starting...");
deferredLogger.LogInformation("Loading configuration from {ConfigPath}", configPath);

// Use scopes just like with regular logging
using (deferredLogger.BeginScope("Service Registration"))
{
    deferredLogger.LogInformation("Registering service {ServiceName}", serviceName);
    deferredLogger.LogDebug("Service configuration: {@Config}", config);
}

// Later, when your logging infrastructure is available
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
logger.FlushDeferredLogs(); // All queued messages are now logged
```

### Integration with Startup Tasks

For ASP.NET Core applications, you can use a startup task to automatically flush deferred logs:

```csharp
namespace MyApp.StartupTasks;

using Cirreum.Logging.Deferred;

internal class FlushDeferredLogs(
    IHostApplicationLifetime lifetime,
    ILogger<FlushDeferredLogs> logger)
    : IStartupTask 
{
    public int Order => int.MaxValue; // Run last
    
    public ValueTask ExecuteAsync() 
    {
        lifetime.ApplicationStarted.Register(HandleApplicationStarted);
        return ValueTask.CompletedTask;
    }
    
    private void HandleApplicationStarted() 
    {
        logger.FlushDeferredLogs();
    }
}
```

### Checking for Errors During Startup

You can also inspect the deferred log queue to check for errors that occurred during startup:

```csharp
// Check if any errors were logged during startup
if (Logger.HasErrors())
{
    var errors = Logger.GetErrors();
    foreach (var error in errors)
    {
        Console.WriteLine($"Startup error: {error}");
    }
}

// Get all entries of a specific level
var warnings = Logger.GetAll(LogLevel.Warning);
foreach (var (level, message) in warnings)
{
    Console.WriteLine($"{level}: {message}");
}

// Get all logged entries
var allEntries = Logger.GetAll();
```

## API Reference

### IDeferredLogger Interface

```csharp
public interface IDeferredLogger 
{
    IDisposable BeginScope<TState>(TState state) where TState : notnull;
    void LogDebug(string message, params object[] args);
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
    void LogError(string message, params object[] args);
    void LogCritical(string message, params object[] args);
    void LogTrace(string message, params object[] args);
}
```

### Logger Static Methods

```csharp
public static class Logger 
{
    // Create a new deferred logger instance
    public static IDeferredLogger CreateDeferredLogger();
    
    // Check for specific log levels
    public static bool HasErrors();
    public static bool HasEntries(LogLevel level);
    
    // Retrieve logged messages
    public static IEnumerable<string> GetErrors();
    public static IEnumerable<(LogLevel Level, string Message)> GetAll(LogLevel level);
    public static IEnumerable<(LogLevel Level, string Message)> GetAll();
}
```

### Extension Methods

```csharp
public static class DeferredLoggerExtensions 
{
    // Flush all queued deferred logs to the provided logger
    public static void FlushDeferredLogs(this ILogger logger);
}
```

## How It Works

1. **Queuing Phase**: During application startup, log messages are queued in memory along with their metadata (level, message template, arguments, and active scopes)

2. **Flushing Phase**: Once the logging infrastructure is available, call `FlushDeferredLogs()` to replay all queued messages through the actual logger

3. **Scope Preservation**: Any logging scopes active when messages were queued are recreated during the flush operation

## Thread Safety

The library is designed to be thread-safe for typical startup scenarios where multiple initialization tasks might be logging concurrently.

## Contributing

This package is part of the Cirreum ecosystem. Follow the established patterns when contributing new features or provider implementations.
