using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace StockApi.Tests.Helpers;
public class TestOutputLoggerProvider : ILoggerProvider
{
    private readonly ITestOutputHelper _output;
    private bool _disposed;

    public TestOutputLoggerProvider(ITestOutputHelper output)
    {
        _output = output;
    }

    public ILogger CreateLogger(string categoryName) => new TestOutputLogger(_output, categoryName);

    private sealed class TestOutputLogger : ILogger
    {
        private readonly ITestOutputHelper _output;
        private readonly string _categoryName;

        public TestOutputLogger(ITestOutputHelper output, string categoryName)
        {
            _output = output;
            _categoryName = categoryName;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            _output.WriteLine($"[{logLevel}] [{_categoryName}] {formatter(state, exception)}");
            if (exception is not null)
            {
                _output.WriteLine(exception.ToString());
            }
        }
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            // If you had any managed resources, dispose them here.
            // No unmanaged resources to clean up in this implementation.
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}