using Microsoft.Extensions.Logging;

namespace MenuApp.Core.Logging;

/// <summary>
///     Logger interface
/// </summary>
public interface ILoggerService
{
    /// <summary>
    ///     Inserts a log item
    /// </summary>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    /// <param name="logLevel">Log level</param>
    /// <returns>A log item</returns>
    Task InsertLog(string message, Exception exception, LogLevel logLevel = LogLevel.Error);
}