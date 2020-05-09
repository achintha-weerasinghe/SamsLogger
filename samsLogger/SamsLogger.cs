using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace samsLogger
{
    public class SamsLogger<TDbContext, TLog> : ILogger
        where TLog : SamsLog, new()
        where TDbContext : DbContext
    {
        readonly string _name;
        readonly string _environment;
        readonly Func<string, LogLevel, bool> _filter;
        IServiceProvider _services;
        public SamsLogger(string name, Func<string, LogLevel, bool> filter, IServiceProvider serviceProvider)
        {
            _name = name;
            _filter = filter ?? GetFilter(serviceProvider.GetService<IOptions<SamsLoggerOptions>>());
            _environment = GetEnvironment(serviceProvider.GetService<IOptions<SamsLoggerOptions>>());
            _services = serviceProvider;
        }

        private string GetEnvironment(IOptions<SamsLoggerOptions> options)
        {
            if (options != null)
            {
                var loggerOptions = options.Value;
                return String.IsNullOrEmpty(loggerOptions.Environment) ? "Development" : loggerOptions.Environment;
            } 
            else
                return "Development";
        }

        private Func<string, LogLevel, bool> GetFilter(IOptions<SamsLoggerOptions> options)
        {
            if (options != null)
            {
                return ((category, level) => GetFilter(options.Value, category, level));
            }
            else
                return ((category, level) => true);
        }

        private bool GetFilter(SamsLoggerOptions options, string category, LogLevel level)
        {
            if (options.Filters != null)
            {
                var filter = options.Filters.Keys.FirstOrDefault(p => category.Contains(p));
                if (filter != null)
                    return (int)options.Filters[filter] <= (int)level;
                else return true;
            }
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new NoopDisposable();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _filter(_name, logLevel);
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var log = new TLog
            {
                Message = Trim(state.ToString(), SamsLog.MaximumMessageLength),
                Date = DateTime.UtcNow,
                Level = logLevel.ToString(),
                Logger = _name,
                Environment = _environment,
                Thread = eventId.ToString()
            };

            if (exception != null)
                log.Exception = Trim(exception.ToString(), SamsLog.MaximumExceptionLength);

            var httpContext = _services.GetRequiredService<IHttpContextAccessor>()?.HttpContext;

            if (httpContext != null)
            {
                log.Browser = httpContext.Request.Headers["User-Agent"];
                log.Username = httpContext.User.Identity.Name ?? "Anonymous";
                try { log.HostAddress = httpContext.Connection.LocalIpAddress?.ToString(); }
                catch (ObjectDisposedException) { log.HostAddress = "Disposed"; }
                log.Url = httpContext.Request.Path;
                log.RequestMethod = httpContext.Request.Method;
            }

            var db = _services.CreateScope().ServiceProvider.GetRequiredService<TDbContext>();
            db.Set<TLog>().Add(log);

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
            }
        }

        private static string Trim(string value, int maximumLength)
        {
            return value.Length > maximumLength ? value.Substring(0, maximumLength) : value;
        }

        private class NoopDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}
