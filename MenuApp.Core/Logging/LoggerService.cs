using MenuApp.Data;
using MenuApp.Data.Entities.System;
using Microsoft.Extensions.Logging;

namespace MenuApp.Core.Logging;

/// <summary>
///     Default logger
/// </summary>
public sealed class LoggerService : ILoggerService
{
    #region Ctor

    public LoggerService(
        ApplicationDbContext dbContext,
        ILogger<LoggerService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    #endregion

    #region Methods

    public async Task InsertLog(string message, Exception exception, LogLevel logLevel = LogLevel.Error)
    {
        try
        {
            _logger.Log(logLevel, message, new
            {
                Message = message,
                Exception = exception.Message
            });

            //save to log file
            var logFilePath = Path.Combine("wwwroot", "log.txt");
            var logText =
                $"[{DateTime.UtcNow}]--[{logLevel}]--{message}--{exception} {Environment.NewLine}{Environment.NewLine}";
            await File.AppendAllTextAsync(logFilePath, logText);

            var log = new Log
            {
                ShortMessage = message,
                FullMessage = exception.Message,
                CreatedOnUtc = DateTime.UtcNow
            };

            await _dbContext.Logs.AddAsync(log);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            await InsertLog(nameof(InsertLog), e);
            throw;
        }
    }

    #endregion

    #region Fields

    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<LoggerService> _logger;

    #endregion
}