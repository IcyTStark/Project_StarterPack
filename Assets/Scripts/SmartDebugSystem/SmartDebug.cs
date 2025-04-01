using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using UnityEngine;

/// <summary>
/// Enhanced debugging utility that provides improved logging capabilities
/// over standard Debug.Log functions with additional features for development.
/// </summary>
public static class SmartDebug
{
    #region Configuration

    // Log level configuration
    public enum LogLevel
    {
        Trace = 0,
        Debug = 1,
        Info = 2,
        Warning = 3,
        Error = 4,
        Fatal = 5,
        None = 6  // Use to disable all logging
    }

    // Current log level threshold - logs below this level won't be displayed
    private static LogLevel _currentLogLevel = LogLevel.Debug;

    // Change the current log level
    public static LogLevel CurrentLogLevel
    {
        get => _currentLogLevel;
        set => _currentLogLevel = value;
    }

    // Channel system for categorizing logs
    private static HashSet<string> _enabledChannels = new HashSet<string> { "DEFAULT" };

    // Enable specific logging channels
    public static void EnableChannel(string channel)
    {
        _enabledChannels.Add(channel.ToUpper());
    }

    // Disable specific logging channels
    public static void DisableChannel(string channel)
    {
        _enabledChannels.Remove(channel.ToUpper());
    }

    // Whether to include timestamps in logs
    public static bool IncludeTimestamp = true;

    // Whether to include the calling method in logs
    public static bool IncludeCallerInfo = true;

    // Whether to include thread ID in logs
    public static bool IncludeThreadInfo = false;

    // Whether to write logs to file
    public static bool LogToFile = false;
    private static string _logFilePath = $"{Application.persistentDataPath}/SmartDebug_{DateTime.Now:yyyyMMdd_HHmmss}.log";

    // Set custom log file path
    public static void SetLogFilePath(string path)
    {
        _logFilePath = path;
    }

    #endregion

    #region Color Definitions

    // ANSI color codes for console output
    private static readonly string TRACE_COLOR = "#AAAAAA"; // Light gray
    private static readonly string DEBUG_COLOR = "#FFFFFF"; // White
    private static readonly string INFO_COLOR = "#00FFFF";  // Cyan
    private static readonly string WARNING_COLOR = "#FFFF00"; // Yellow
    private static readonly string ERROR_COLOR = "#FF6666"; // Light red
    private static readonly string FATAL_COLOR = "#FF0000"; // Bright red
    private static readonly string PERF_COLOR = "#00FF00";  // Green
    private static readonly string MEMORY_COLOR = "#FFA500"; // Orange
    private static readonly string HIGHLIGHT_COLOR = "#FF00FF"; // Magenta

    #endregion

    #region Basic Logging Methods

    /// <summary>
    /// Log a trace message (lowest level, high detail)
    /// </summary>
    [Conditional("ENABLE_LOGGING")]
    public static void Trace(object message, string channel = "DEFAULT",
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
    {
        Log(message, LogLevel.Trace, channel, TRACE_COLOR, callerName, callerFilePath, callerLineNumber);
    }

    /// <summary>
    /// Log a debug message (development info)
    /// </summary>
    [Conditional("ENABLE_LOGGING")]
    public static void Log(object message, string channel = "DEFAULT",
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
    {
        Log(message, LogLevel.Debug, channel, DEBUG_COLOR, callerName, callerFilePath, callerLineNumber);
    }

    /// <summary>
    /// Log an info message (general information)
    /// </summary>
    [Conditional("ENABLE_LOGGING")]
    public static void Info(object message, string channel = "DEFAULT",
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
    {
        Log(message, LogLevel.Info, channel, INFO_COLOR, callerName, callerFilePath, callerLineNumber);
    }

    /// <summary>
    /// Log a warning message (potential issues)
    /// </summary>
    [Conditional("ENABLE_LOGGING")]
    public static void Warning(object message, string channel = "DEFAULT",
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
    {
        Log(message, LogLevel.Warning, channel, WARNING_COLOR, callerName, callerFilePath, callerLineNumber);
    }

    /// <summary>
    /// Log an error message with stack trace (non-fatal errors)
    /// </summary>
    [Conditional("ENABLE_LOGGING")]
    public static void Error(object message, string channel = "DEFAULT",
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
    {
        // Create stack trace (skip 1 frame to exclude this method call)
        StackTrace stackTrace = new StackTrace(1, true);
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(message.ToString());
        sb.AppendLine("Stack trace:");

        foreach (StackFrame frame in stackTrace.GetFrames())
        {
            string fileName = Path.GetFileName(frame.GetFileName() ?? "Unknown");
            string methodName = frame.GetMethod()?.Name ?? "Unknown";
            int line = frame.GetFileLineNumber();

            sb.AppendLine($"  at {methodName} in {fileName}:line {line}");
        }

        Log(sb.ToString(), LogLevel.Error, channel, ERROR_COLOR, callerName, callerFilePath, callerLineNumber);
    }

    /// <summary>
    /// Log a fatal error message with stack trace (critical issues)
    /// </summary>
    [Conditional("ENABLE_LOGGING")]
    public static void Fatal(object message, string channel = "DEFAULT",
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
    {
        // Create stack trace (skip 1 frame to exclude this method call)
        StackTrace stackTrace = new StackTrace(1, true);
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(message.ToString());
        sb.AppendLine("Stack trace:");

        foreach (StackFrame frame in stackTrace.GetFrames())
        {
            string fileName = Path.GetFileName(frame.GetFileName() ?? "Unknown");
            string methodName = frame.GetMethod()?.Name ?? "Unknown";
            int line = frame.GetFileLineNumber();

            sb.AppendLine($"  at {methodName} in {fileName}:line {line}");
        }

        Log(sb.ToString(), LogLevel.Fatal, channel, FATAL_COLOR, callerName, callerFilePath, callerLineNumber);
    }

    /// <summary>
    /// Generic log method with level and color
    /// </summary>
    private static void Log(object message, LogLevel level, string channel, string color,
        string callerName, string callerFilePath, int callerLineNumber)
    {
        // Check if we should log based on level and channel
        if (level < _currentLogLevel || !_enabledChannels.Contains(channel.ToUpper()))
            return;

        StringBuilder logBuilder = new StringBuilder();

        // Add timestamp if enabled
        if (IncludeTimestamp)
            logBuilder.Append($"[{DateTime.Now:HH:mm:ss.fff}] ");

        // Add log level
        logBuilder.Append($"[{level}] ");

        // Add channel if not default
        if (channel != "DEFAULT")
            logBuilder.Append($"[{channel}] ");

        // Add thread info if enabled
        if (IncludeThreadInfo)
            logBuilder.Append($"[Thread:{Thread.CurrentThread.ManagedThreadId}] ");

        // Add caller info if enabled
        if (IncludeCallerInfo)
        {
            string fileName = Path.GetFileName(callerFilePath);
            logBuilder.Append($"[{fileName}:{callerName}:{callerLineNumber}] ");
        }

        // Add the actual message
        logBuilder.Append(message);

        string finalMessage = logBuilder.ToString();

        // Log to Unity console with appropriate color and method
        switch (level)
        {
            case LogLevel.Warning:
                UnityEngine.Debug.LogWarning($"<color={color}>{finalMessage}</color>");
                break;
            case LogLevel.Error:
            case LogLevel.Fatal:
                UnityEngine.Debug.LogError($"<color={color}>{finalMessage}</color>");
                break;
            default:
                UnityEngine.Debug.Log($"<color={color}>{finalMessage}</color>");
                break;
        }

        // Also log to file if enabled
        if (LogToFile)
        {
            try
            {
                File.AppendAllText(_logFilePath, finalMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // Fall back to Unity logging if file writing fails
                UnityEngine.Debug.LogError($"Failed to write to log file: {ex.Message}");
            }
        }
    }

    #endregion

    #region Advanced Logging Features

    /// <summary>
    /// Log the values of multiple variables or expressions at once
    /// </summary>
    [Conditional("ENABLE_LOGGING")]
    public static void Variables(string channel = "DEFAULT", params (string name, object value)[] variables)
    {
        if (LogLevel.Debug < _currentLogLevel || !_enabledChannels.Contains(channel.ToUpper()))
            return;

        StringBuilder message = new StringBuilder("Variables:");
        foreach (var (name, value) in variables)
        {
            message.Append($"\n  {name} = {(value?.ToString() ?? "null")}");
        }

        Log(message.ToString(), LogLevel.Debug, channel, DEBUG_COLOR, "", "", 0);
    }

    /// <summary>
    /// Highlight a message with a distinctive color for emphasis
    /// </summary>
    [Conditional("ENABLE_LOGGING")]
    public static void Highlight(object message, string channel = "DEFAULT")
    {
        Log(message, LogLevel.Info, channel, HIGHLIGHT_COLOR, "", "", 0);
    }

    /// <summary>
    /// Log only in development builds, not in releases
    /// </summary>
    [Conditional("DEVELOPMENT_BUILD")]
    [Conditional("UNITY_EDITOR")]
    public static void DevOnly(object message, string channel = "DEFAULT")
    {
        Log(message, LogLevel.Debug, channel, DEBUG_COLOR, "", "", 0);
    }

    /// <summary>
    /// Log an object's properties and fields
    /// </summary>
    [Conditional("ENABLE_LOGGING")]
    public static void Dump(object obj, string objName = "Object", string channel = "DEFAULT")
    {
        if (obj == null)
        {
            Log($"{objName} is null", LogLevel.Warning, channel, WARNING_COLOR, "", "", 0);
            return;
        }

        Type type = obj.GetType();
        StringBuilder details = new StringBuilder();
        details.AppendLine($"{objName} ({type.Name}) Dump:");

        // Get and display properties
        var properties = type.GetProperties();
        if (properties.Length > 0)
        {
            details.AppendLine("  Properties:");
            foreach (var prop in properties)
            {
                try
                {
                    var value = prop.GetValue(obj);
                    details.AppendLine($"    {prop.Name} = {(value?.ToString() ?? "null")}");
                }
                catch (Exception ex)
                {
                    details.AppendLine($"    {prop.Name} = <Error: {ex.Message}>");
                }
            }
        }

        // Get and display fields
        var fields = type.GetFields();
        if (fields.Length > 0)
        {
            details.AppendLine("  Fields:");
            foreach (var field in fields)
            {
                try
                {
                    var value = field.GetValue(obj);
                    details.AppendLine($"    {field.Name} = {(value?.ToString() ?? "null")}");
                }
                catch (Exception ex)
                {
                    details.AppendLine($"    {field.Name} = <Error: {ex.Message}>");
                }
            }
        }

        Log(details.ToString(), LogLevel.Debug, channel, DEBUG_COLOR, "", "", 0);
    }

    #endregion

    #region Performance Tracking

    // Dictionary to store timing information
    private static Dictionary<string, Stopwatch> _timers = new Dictionary<string, Stopwatch>();
    private static Dictionary<string, List<double>> _timerHistory = new Dictionary<string, List<double>>();

    /// <summary>
    /// Start a performance timer with the given identifier
    /// </summary>
    [Conditional("ENABLE_LOGGING")]
    public static void StartTimer(string timerId)
    {
        if (!_timers.TryGetValue(timerId, out Stopwatch timer))
        {
            timer = new Stopwatch();
            _timers[timerId] = timer;
        }

        if (!_timerHistory.ContainsKey(timerId))
        {
            _timerHistory[timerId] = new List<double>();
        }

        timer.Restart();
    }

    /// <summary>
    /// Stop a performance timer and log the elapsed time
    /// </summary>
    [Conditional("ENABLE_LOGGING")]
    public static void StopTimer(string timerId, string message = null, string channel = "PERFORMANCE")
    {
        if (!_timers.TryGetValue(timerId, out Stopwatch timer))
        {
            Log($"Timer '{timerId}' was not started", LogLevel.Warning, channel, WARNING_COLOR, "", "", 0);
            return;
        }

        timer.Stop();
        double elapsed = timer.Elapsed.TotalMilliseconds;
        _timerHistory[timerId].Add(elapsed);

        string logMessage = message != null
            ? $"{message}: {elapsed:F3} ms"
            : $"Timer '{timerId}': {elapsed:F3} ms";

        Log(logMessage, LogLevel.Debug, channel, PERF_COLOR, "", "", 0);
    }

    /// <summary>
    /// Get statistics for a specific timer
    /// </summary>
    [Conditional("ENABLE_LOGGING")]
    public static void TimerStats(string timerId, string channel = "PERFORMANCE")
    {
        if (!_timerHistory.TryGetValue(timerId, out List<double> history) || history.Count == 0)
        {
            Log($"No history for timer '{timerId}'", LogLevel.Warning, channel, WARNING_COLOR, "", "", 0);
            return;
        }

        double min = history.Min();
        double max = history.Max();
        double avg = history.Average();
        double sum = history.Sum();

        StringBuilder stats = new StringBuilder();
        stats.AppendLine($"Timer '{timerId}' Statistics:");
        stats.AppendLine($"  Count: {history.Count}");
        stats.AppendLine($"  Min: {min:F3} ms");
        stats.AppendLine($"  Max: {max:F3} ms");
        stats.AppendLine($"  Avg: {avg:F3} ms");
        stats.AppendLine($"  Total: {sum:F3} ms");

        Log(stats.ToString(), LogLevel.Debug, channel, PERF_COLOR, "", "", 0);
    }

    /// <summary>
    /// Profile a block of code and measure its execution time
    /// </summary>
    public static void Profile(Action action, string label = "Code Block", string channel = "PERFORMANCE")
    {
        Stopwatch sw = Stopwatch.StartNew();
        try
        {
            action();
        }
        finally
        {
            sw.Stop();
            Log($"{label}: {sw.Elapsed.TotalMilliseconds:F3} ms", LogLevel.Debug, channel, PERF_COLOR, "", "", 0);
        }
    }

    /// <summary>
    /// Profile a function that returns a value and measure its execution time
    /// </summary>
    public static T Profile<T>(Func<T> function, string label = "Function Call", string channel = "PERFORMANCE")
    {
        Stopwatch sw = Stopwatch.StartNew();
        try
        {
            return function();
        }
        finally
        {
            sw.Stop();
            Log($"{label}: {sw.Elapsed.TotalMilliseconds:F3} ms", LogLevel.Debug, channel, PERF_COLOR, "", "", 0);
        }
    }

    #endregion

    #region Memory Tracking

    private static long _lastMemory = 0;

    /// <summary>
    /// Log the current memory usage of the application
    /// </summary>
    [Conditional("ENABLE_LOGGING")]
    public static void LogMemoryUsage(string message = null, string channel = "MEMORY")
    {
        long currentMemory = GC.GetTotalMemory(false);
        long memoryDelta = currentMemory - _lastMemory;
        _lastMemory = currentMemory;

        StringBuilder memMessage = new StringBuilder();
        if (message != null)
            memMessage.Append($"{message}: ");

        memMessage.Append($"Memory: {FormatBytes(currentMemory)}");

        if (memoryDelta != 0)
        {
            string sign = memoryDelta > 0 ? "+" : "";
            memMessage.Append($" ({sign}{FormatBytes(memoryDelta)})");
        }

        Log(memMessage.ToString(), LogLevel.Debug, channel, MEMORY_COLOR, "", "", 0);
    }

    /// <summary>
    /// Format bytes into a readable string
    /// </summary>
    private static string FormatBytes(long bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB" };
        int i = 0;
        double dblBytes = Math.Abs(bytes);

        while (dblBytes >= 1024 && i < suffixes.Length - 1)
        {
            dblBytes /= 1024;
            i++;
        }

        return $"{dblBytes:F2} {suffixes[i]}";
    }

    /// <summary>
    /// Force a garbage collection and log memory before and after
    /// </summary>
    [Conditional("ENABLE_LOGGING")]
    public static void ForceGC(string channel = "MEMORY")
    {
        long before = GC.GetTotalMemory(false);
        Log($"Before GC: {FormatBytes(before)}", LogLevel.Debug, channel, MEMORY_COLOR, "", "", 0);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long after = GC.GetTotalMemory(true);
        long reclaimed = before - after;

        Log($"After GC: {FormatBytes(after)} (Reclaimed: {FormatBytes(reclaimed)})",
            LogLevel.Debug, channel, MEMORY_COLOR, "", "", 0);
    }

    #endregion

    #region Assertion Helpers

    /// <summary>
    /// Assert that a condition is true, with optional message
    /// </summary>
    [Conditional("ENABLE_LOGGING")]
    public static void Assert(bool condition, string message = "Assertion failed", string channel = "ASSERT",
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (!condition)
        {
            Log(message, LogLevel.Error, channel, ERROR_COLOR, callerName, callerFilePath, callerLineNumber);
            if (UnityEngine.Debug.isDebugBuild)
            {
                UnityEngine.Debug.Break(); // Pause the editor if in debug mode
            }
        }
    }

    /// <summary>
    /// Assert that an object is not null
    /// </summary>
    [Conditional("ENABLE_LOGGING")]
    public static void AssertNotNull(object obj, string name = "Object", string channel = "ASSERT",
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (obj == null)
        {
            Log($"{name} is null", LogLevel.Error, channel, ERROR_COLOR, callerName, callerFilePath, callerLineNumber);
            if (UnityEngine.Debug.isDebugBuild)
            {
                UnityEngine.Debug.Break();
            }
        }
    }

    #endregion

    #region Stack Trace Features

    /// <summary>
    /// Log the current stack trace with customizable depth
    /// </summary>
    [Conditional("ENABLE_LOGGING")]
    public static void LogStackTrace(string message = "Stack Trace:", int skipFrames = 1, int maxFrames = 0, string channel = "DEFAULT")
    {
        StackTrace stackTrace = new StackTrace(skipFrames, true);
        StackFrame[] frames = stackTrace.GetFrames();

        StringBuilder sb = new StringBuilder();
        sb.AppendLine(message);

        int framesToLog = maxFrames > 0 ? Math.Min(maxFrames, frames.Length) : frames.Length;

        for (int i = 0; i < framesToLog; i++)
        {
            StackFrame frame = frames[i];
            string fileName = Path.GetFileName(frame.GetFileName() ?? "Unknown");
            string methodName = frame.GetMethod()?.Name ?? "Unknown";
            int line = frame.GetFileLineNumber();

            sb.AppendLine($"  at {methodName} in {fileName}:line {line}");
        }

        Log(sb.ToString(), LogLevel.Debug, channel, DEBUG_COLOR, "", "", 0);
    }

    /// <summary>
    /// Log a provided exception with its stack trace
    /// </summary>
    [Conditional("ENABLE_LOGGING")]
    public static void LogException(Exception exception, string message = null, string channel = "DEFAULT")
    {
        StringBuilder sb = new StringBuilder();

        if (!string.IsNullOrEmpty(message))
            sb.AppendLine(message);

        sb.AppendLine($"Exception: {exception.GetType().Name}: {exception.Message}");
        sb.AppendLine(exception.StackTrace);

        // Include inner exception if available
        if (exception.InnerException != null)
        {
            sb.AppendLine($"Inner Exception: {exception.InnerException.GetType().Name}: {exception.InnerException.Message}");
            sb.AppendLine(exception.InnerException.StackTrace);
        }

        Log(sb.ToString(), LogLevel.Error, channel, ERROR_COLOR, "", "", 0);
    }

    /// <summary>
    /// Try to execute an action and log any exceptions that occur
    /// </summary>
    public static void TryCatch(Action action, string actionName = "Operation", string channel = "DEFAULT")
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            LogException(ex, $"Exception in {actionName}", channel);
        }
    }

    /// <summary>
    /// Try to execute a function and log any exceptions that occur
    /// </summary>
    public static T TryCatch<T>(Func<T> function, T defaultValue = default, string functionName = "Function", string channel = "DEFAULT")
    {
        try
        {
            return function();
        }
        catch (Exception ex)
        {
            LogException(ex, $"Exception in {functionName}", channel);
            return defaultValue;
        }
    }

    #endregion

    #region Convenience Methods

    /// <summary>
    /// Clear the log file
    /// </summary>
    public static void ClearLogFile()
    {
        if (LogToFile && File.Exists(_logFilePath))
        {
            File.WriteAllText(_logFilePath, string.Empty);
        }
    }

    /// <summary>
    /// Set up SmartDebug with common configuration
    /// </summary>
    public static void Configure(LogLevel logLevel = LogLevel.Debug, bool logToFile = false, bool includeTimestamp = true, bool includeCallerInfo = true)
    {
        _currentLogLevel = logLevel;
        LogToFile = logToFile;
        IncludeTimestamp = includeTimestamp;
        IncludeCallerInfo = includeCallerInfo;

        if (LogToFile)
        {
            // Create directory if it doesn't exist
            string directory = Path.GetDirectoryName(_logFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Add header to the log file
            File.WriteAllText(_logFilePath, $"=== SmartDebug Log Started at {DateTime.Now} ===\n");
        }

        // Log the startup configuration
        Info($"SmartDebug initialized with LogLevel: {_currentLogLevel}, LogToFile: {LogToFile}", "SYSTEM");
    }

    #endregion
}