using System;
using NLog;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure.Testing
{
    public static class LoggerExtensions
    {
        public static void Trace([NotNull] this string message, [NotNull] ILogger logger) 
            => logger.Trace(message);

        public static void Info([NotNull] this string message, [NotNull] ILogger logger) 
            => logger.Info(message);

        public static void Debug([NotNull] this string message, [NotNull] ILogger logger) 
            => logger.Debug(message);

        public static void Warn([NotNull] this string message, [NotNull] ILogger logger) 
            => logger.Warn(message);

        public static void Trace([NotNull] this Exception x, [NotNull] ILogger logger) 
            => logger.Error(x);
    }
}
