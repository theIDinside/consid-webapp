using Microsoft.Extensions.Options;

namespace webapp.mvc.Loggers
{

    // RoundTheCodeFileLoggerOptions.cs
    public class FileLoggerOptions
    {
        public virtual string filePath { get; set; }

        public virtual string folderPath { get; set; }
    }

    public class FileLogger : ILogger
    {
        private readonly string m_loggerName;
        protected readonly FileLoggerProvider m_provider;
        public FileLogger(string name, FileLoggerProvider loggerProvider)
        {
            m_loggerName = name;
            m_provider = loggerProvider;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return new NoopDisposable();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                Task.Run(async () =>
                {
                    var fullFilePath = m_provider.m_options.folderPath + "/" + m_provider.m_options.filePath.Replace("{name}", string.Format("{0}_{1}", DateTimeOffset.UtcNow.ToString("yyyyMMdd"), m_loggerName));
                    var logRecord = string.Format("{0} [{1}] {2} {3}", "[" + DateTimeOffset.UtcNow.ToString("yyyy-MM-dd (HH:mm:ss.fff)") + "]", logLevel.ToString(), formatter(state, exception), exception != null ? exception.StackTrace : "");
                    try
                    {
                        await m_provider.m_fileLock.WaitAsync();
                        using (var streamWriter = new StreamWriter(fullFilePath, true))
                        {
                            streamWriter.WriteLine(logRecord);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(string.Format("Logger {0} FAILED LOGGING TO FILE. [Exception]:{1}", m_loggerName, e.ToString()));
                    }
                    finally
                    {
                        // finally branches are of utmost importance when acquiring locks, this goes for any language, because if an exception throws
                        // we will create a dead lock
                        Console.WriteLine($"{m_loggerName} logged to file {m_provider.m_options.filePath}");
                        m_provider.m_fileLock.Release();
                    }
                });
            }
        }

        // no one should use this class but me. We just say "don't care about disposing."
        private class NoopDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }

    [ProviderAlias("FileLogger")]
    public class FileLoggerProvider : ILoggerProvider
    {
        public readonly FileLoggerOptions m_options;
        // grab a "cheap" mutex (mutexes are never cheap, but they are necessary). Since our logging is asynchronous, we have to guard at potentially 
        // having multiple threads writing to our file logger
        public SemaphoreSlim m_fileLock;

        public FileLoggerProvider(IOptions<FileLoggerOptions> options)
        {
            m_fileLock = new SemaphoreSlim(1, 1);
            m_options = options.Value;
            if (!Directory.Exists(m_options.folderPath))
            {
                Directory.CreateDirectory(m_options.folderPath);
            }
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(categoryName, this);
        }

        public void Dispose() { }
    }

    // Extension for ILoggingBuilder so that we can say cfg.AddColorFileLogger() in Program.cs file
    public static class LoggingBuilderExtension
    {
        public static ILoggingBuilder AddFileLogger(this ILoggingBuilder builder, Action<FileLoggerOptions> configure)
        {
            builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>();
            builder.Services.Configure(configure);
            return builder;
        }
    }
}