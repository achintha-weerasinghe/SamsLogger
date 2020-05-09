using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace samsLogger
{
    public class SamsLoggerOptions
    {
        public Dictionary<string, LogLevel> Filters { get; set; }
        public string Environment { get; set; }
    }
}