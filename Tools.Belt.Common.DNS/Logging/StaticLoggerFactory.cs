using Microsoft.Extensions.Logging;

namespace Tools.Belt.Common.Logging
{
    public static class StaticLoggerFactory
    {
        public static ILogger CreateLogger(string categoryName)
        {
            using LoggerFactory factory = new LoggerFactory();
            return factory.CreateLogger(categoryName);
        }

        public static ILogger CreateLogger<T>()
        {
            using LoggerFactory factory = new LoggerFactory();
            return factory.CreateLogger<T>();
        }

        public static ILogger<T> CreateObjectLogger<T>(this T source)
        {
            using LoggerFactory factory = new LoggerFactory();
            return factory.CreateLogger<T>();
        }
    }
}