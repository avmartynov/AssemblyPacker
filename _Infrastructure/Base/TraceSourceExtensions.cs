using System;
using System.Diagnostics;
using System.Reflection;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure
{
    public static class TraceSourceExtensions
    {
        public static void Trace([NotNull] this TraceSource traceSource, [NotNull] string message)
        {
            if (traceSource == null)
                throw new ArgumentNullException(nameof(traceSource));

            traceSource.TraceEvent(TraceEventType.Verbose, 0, message);    
        }


        public static void Trace([NotNull] this string message, [NotNull] TraceSource traceSource)
            => traceSource.Trace(message);


        public static void Trace([NotNull] this TraceSource traceSource, [NotNull] string format, [NotNull] params object[] args)
        {
            if (traceSource == null)
                throw new ArgumentNullException(nameof(traceSource));

            traceSource.TraceEvent(TraceEventType.Verbose, 0, format, args);    
        }


        public static void Info([NotNull] this TraceSource traceSource, [NotNull] string message)
        {
            if (traceSource == null)
                throw new ArgumentNullException(nameof(traceSource));

            traceSource.TraceEvent(TraceEventType.Information, 0, message);    
        }


        // public static void Info([NotNull] this string message, [NotNull] TraceSource traceSource)
           // => traceSource.Info(message);


        public static void Info([NotNull] this TraceSource traceSource, [NotNull] string format, [NotNull] params object[] args)
        {
            if (traceSource == null)
                throw new ArgumentNullException(nameof(traceSource));

            traceSource.TraceEvent(TraceEventType.Information, 0, format, args);    
        }


        public static void Warn([NotNull] this TraceSource traceSource, [NotNull] string message)
        {
            if (traceSource == null)
                throw new ArgumentNullException(nameof(traceSource));

            traceSource.TraceEvent(TraceEventType.Warning, 0, message);    
        }


        public static void Warn([NotNull] this string message, [NotNull] TraceSource traceSource)
            => traceSource.Warn(message);


        public static void Error([NotNull] this TraceSource traceSource, [NotNull] string format, [NotNull]params object[] args)
        {
            if (traceSource == null)
                throw new ArgumentNullException(nameof(traceSource));

            traceSource.TraceEvent(TraceEventType.Error, 0, format, args);    
        }


        public static void Error([NotNull] this TraceSource traceSource, [NotNull] string message)
        {
            if (traceSource == null)
                throw new ArgumentNullException(nameof(traceSource));

            traceSource.TraceEvent(TraceEventType.Error, 0, message);    
        }


        public static void Error([NotNull] this string message, [NotNull] TraceSource traceSource)
            => traceSource.Error(message);


        public static void Error([NotNull] this TraceSource traceSource, [NotNull] Exception exception, [CanBeNull] string message = null)
        {
            if (traceSource == null)
                throw new ArgumentNullException(nameof(traceSource));

            traceSource.TraceEvent(TraceEventType.Error, 0, $"{message}:{Environment.NewLine}{exception}");    
        }


        public static void Log([NotNull] this Exception exception, [NotNull] TraceSource traceSource)
            => traceSource.Error(exception);


        public static void Log([NotNull] this TraceSource traceSource, SourceLevels logLevel, [NotNull] string message)
        {
            switch (logLevel)
            {
                case SourceLevels.Error:       traceSource.Error(message); break;
                case SourceLevels.Warning:     traceSource.Warn(message);  break;
                case SourceLevels.Information: traceSource.Info(message);  break;
                case SourceLevels.Verbose:     traceSource.Trace(message); break;
            }
        }
        

        public static void Log([NotNull] this string message, [NotNull] TraceSource traceSource, SourceLevels logLevel = SourceLevels.Information)
            => traceSource.Log(logLevel, message);


        [NotNull]
        public static TraceSource GetTraceSource([NotNull] this MethodBase method)
            => new TraceSource(method.DeclaringType?.Namespace ?? "");
    }
}
