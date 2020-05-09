using System;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace samsLogger
{
    public class SamsLoggerProvider<TDbContext, TLog> : ILoggerProvider
        where TDbContext : DbContext
        where TLog : SamsLog, new()
    {
        readonly Func<string, LogLevel, bool> _filter;
        readonly IServiceProvider _serviceProvider;
        public SamsLoggerProvider(IServiceProvider serviceProvider, Func<string, LogLevel, bool> filter = null)
        {
            _filter = filter;
            _serviceProvider = serviceProvider;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new SamsLogger<TDbContext, TLog>(categoryName, _filter, _serviceProvider);
        }

        public void Dispose() {}
    }
}