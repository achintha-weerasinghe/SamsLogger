using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace samsLogger
{
    public static class SamsLoggerFactoryExtensions
    {
        public static ILoggerFactory AddSamsLogging<TDbContext, TLog>(
            this ILoggerFactory factory,
            IServiceProvider serviceProvider,
            Func<string, LogLevel, bool> filter = null
        )
            where TDbContext : DbContext
            where TLog : SamsLog, new()
        {
            if (factory == null) throw new ArgumentNullException("factory");
            factory.AddProvider(new SamsLoggerProvider<TDbContext, TLog>(serviceProvider, filter));
            return factory;
        }
    }
}