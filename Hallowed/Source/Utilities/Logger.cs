using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Hallowed.Utilities;

/// <summary>
/// Static logger class with support for log levels, log-once functionality, and formatting
/// </summary>
public static class Logger
{
    private static readonly HashSet<string> LoggedOnce = [];
    
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }
    
    /// <summary>
    /// Minimum log level to display. Messages below this level are ignored.
    /// </summary>
    public static LogLevel MinimumLevel { get; set; } = LogLevel.Debug;
    
    /// <summary>
    /// Enable or disable timestamps in log messages
    /// </summary>
    public static bool ShowTimestamps { get; set; } = false;
    
    #region Basic Logging
    
    /// <summary>
    /// Log a message with specified level
    /// </summary>
    public static void Log(string message, LogLevel level = LogLevel.Info)
    {
        if (level < MinimumLevel) return;
        
        var prefix = GetPrefix(level);
        var timestamp = ShowTimestamps ? $"[{DateTime.Now:HH:mm:ss.fff}] " : "";
        
        System.Diagnostics.Debug.WriteLine($"{timestamp}{prefix} {message}");
    }
    
    /// <summary>
    /// Log a formatted message with specified level
    /// </summary>
    public static void Log(LogLevel level, string format, params object[] args)
    {
        if (level < MinimumLevel) return;
        
        var message = string.Format(format, args);
        Log(message, level);
    }
    
    #endregion
    
    #region Log Once
    
    /// <summary>
    /// Log a message only once (per unique message content)
    /// </summary>
    public static void LogOnce(string message, LogLevel level = LogLevel.Info)
    {
        if (LoggedOnce.Contains(message)) return;
        
        LoggedOnce.Add(message);
        Log(message, level);
    }
    
    /// <summary>
    /// Log a formatted message only once (per unique message content)
    /// </summary>
    public static void LogOnce(LogLevel level, string format, params object[] args)
    {
        var message = string.Format(format, args);
        LogOnce(message, level);
    }
    
    /// <summary>
    /// Log a message only once per call site (uses caller location info)
    /// Useful when you want to log once per code location, regardless of message content
    /// </summary>
    public static void LogOncePerLocation(
        string message, 
        LogLevel level = LogLevel.Info,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "")
    {
        var key = $"{file}:{line}:{member}";
        
        if (LoggedOnce.Contains(key)) return;
        
        LoggedOnce.Add(key);
        var location = $"{System.IO.Path.GetFileName(file)}:{line} in {member}()";
        Log($"{message} [at {location}]", level);
    }
    
    /// <summary>
    /// Clear the log-once cache. Useful when transitioning scenes or resetting state.
    /// </summary>
    [Conditional("DEBUG")]
    public static void ClearOnceCache()
    {
        LoggedOnce.Clear();
    }
    
    #endregion
    
    #region Convenience Methods (Log Level Shortcuts)
    
    // Basic logging - Debug methods removed in Release
    [Conditional("DEBUG")]
    public static void Debug(string message) => Log(message, LogLevel.Debug);
    
    public static void Info(string message) => Log(message);
    public static void Warning(string message) => Log(message, LogLevel.Warning);
    public static void Error(string message) => Log(message, LogLevel.Error);
    
    // Formatted logging - Debug methods removed in Release
    [Conditional("DEBUG")]
    public static void Debug(string format, params object[] args) => Log(LogLevel.Debug, format, args);
    
    public static void Info(string format, params object[] args) => Log(LogLevel.Info, format, args);
    public static void Warning(string format, params object[] args) => Log(LogLevel.Warning, format, args);
    public static void Error(string format, params object[] args) => Log(LogLevel.Error, format, args);
    
    // Log once variants - Debug methods removed in Release
    [Conditional("DEBUG")]
    public static void DebugOnce(string message) => LogOnce(message, LogLevel.Debug);
    
    public static void InfoOnce(string message) => LogOnce(message);
    public static void WarningOnce(string message) => LogOnce(message, LogLevel.Warning);
    public static void ErrorOnce(string message) => LogOnce(message, LogLevel.Error);
    
    // Log once formatted variants - Debug methods removed in Release
    [Conditional("DEBUG")]
    public static void DebugOnce(string format, params object[] args) => LogOnce(LogLevel.Debug, format, args);
    
    public static void InfoOnce(string format, params object[] args) => LogOnce(LogLevel.Info, format, args);
    public static void WarningOnce(string format, params object[] args) => LogOnce(LogLevel.Warning, format, args);
    public static void ErrorOnce(string format, params object[] args) => LogOnce(LogLevel.Error, format, args);
    
    #endregion
    
    #region Utility Methods
    
    /// <summary>
    /// Log with timestamp regardless of ShowTimestamps setting
    /// </summary>
    [Conditional("DEBUG")]
    public static void LogWithTime(string message, LogLevel level = LogLevel.Debug)
    {
        if (level < MinimumLevel) return;
        
        var prefix = GetPrefix(level);
        System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {prefix} {message}");
    }
    
    /// <summary>
    /// Log an exception with full details
    /// </summary>
    public static void LogException(Exception ex, string context = "")
    {
        var contextStr = string.IsNullOrEmpty(context) ? "" : $" [{context}]";
        Error($"Exception{contextStr}: {ex.GetType().Name}: {ex.Message}");
        
        #if DEBUG
        Log($"Stack trace: {ex.StackTrace}", LogLevel.Debug);
        
        if (ex.InnerException != null)
        {
            Error($"Inner exception: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
        }
        #endif
    }
    
    /// <summary>
    /// Log object properties for debugging (DEBUG only)
    /// </summary>
    [Conditional("DEBUG")]
    public static void LogObject(object obj, string label = "Object")
    {
        if (obj == null)
        {
            Debug($"{label}: null");
            return;
        }
        
        Debug($"{label} ({obj.GetType().Name}):");
        var properties = obj.GetType().GetProperties();
        foreach (var prop in properties)
        {
            try
            {
                var value = prop.GetValue(obj);
                Debug($"  {prop.Name} = {value}");
            }
            catch (Exception ex)
            {
                Debug($"  {prop.Name} = [Error: {ex.Message}]");
            }
        }
    }
    
    /// <summary>
    /// Assert a condition and log an error if false (DEBUG only)
    /// </summary>
    [Conditional("DEBUG")]
    public static void Assert(bool condition, string message)
    {
        if (!condition)
        {
            Error($"ASSERTION FAILED: {message}");
            System.Diagnostics.Debug.Assert(condition, message);
        }
    }
    
    /// <summary>
    /// Start a performance timer (DEBUG only)
    /// Returns a Stopwatch in DEBUG, null in RELEASE
    /// </summary>
    public static Stopwatch StartTimer(string label = "")
    {
        #if DEBUG
        var sw = Stopwatch.StartNew();
        if (!string.IsNullOrEmpty(label))
        {
            Debug($"Timer started: {label}");
        }
        return sw;
        #else
        return null;
        #endif
    }
    
    /// <summary>
    /// Stop a performance timer and log the elapsed time (DEBUG only)
    /// </summary>
    [Conditional("DEBUG")]
    public static void StopTimer(Stopwatch sw, string label = "Operation")
    {
        if (sw == null) return;
        sw.Stop();
        Debug($"{label} completed in {sw.ElapsedMilliseconds}ms");
    }
    
    /// <summary>
    /// Log detailed performance timing with memory info (DEBUG only)
    /// </summary>
    [Conditional("DEBUG")]
    public static void LogPerformance(string operation, long milliseconds)
    {
        var memoryMb = GC.GetTotalMemory(false) / 1024.0 / 1024.0;
        Debug($"[PERF] {operation}: {milliseconds}ms (Memory: {memoryMb:F2}MB)");
    }
    
    #endregion
    
    #region Private Helpers
    
    private static string GetPrefix(LogLevel level)
    {
        return level switch
        {
            LogLevel.Debug => "[DEBUG]",
            LogLevel.Info => "[INFO]",
            LogLevel.Warning => "[WARN]",
            LogLevel.Error => "[ERROR]",
            _ => "[LOG]"
        };
    }
    
    #endregion
}