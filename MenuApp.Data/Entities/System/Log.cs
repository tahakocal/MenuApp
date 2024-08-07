using MenuApp.Data.Entities.Base;

namespace MenuApp.Data.Entities.System;

/// <summary>
///     Represents a log record
/// </summary>
public class Log : IEntity
{
    /// <summary>
    ///     Gets or sets the log level identifier
    /// </summary>
    public int LogLevelId { get; set; }

    /// <summary>
    ///     Gets or sets the short message
    /// </summary>
    public string ShortMessage { get; set; }

    /// <summary>
    ///     Gets or sets the full exception
    /// </summary>
    public string FullMessage { get; set; }

    /// <summary>
    ///     Gets or sets the date and time of instance creation
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }
}