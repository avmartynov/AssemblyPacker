using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using NLog;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure.Diagnostics
{
    public class TestFacade
    {
        public TestFacade(string fixtureName)
            => _fixtureName = fixtureName;


        [MethodImpl(MethodImplOptions.NoInlining)]
        [NotNull]
        public static TestFacade GetCurrentClassFacade()
            => new TestFacade(GetCallingClassName());


        public void Execute(Action methodBody, [CanBeNull] [CallerMemberName] string methodName = null)
            => TestBoundary.Execute(_fixtureName, methodName, () => Invoke(methodBody));


        public void Execute<TParams>(TParams parameters, Action methodBody, [CanBeNull] [CallerMemberName] string methodName = null)
            => TestBoundary.Execute(_fixtureName, methodName, parameters, () => Invoke(methodBody));


        #region Private members

        private void Invoke([NotNull] Action methodBody) 
            => (methodBody ?? throw new ArgumentNullException(nameof(methodBody)))();


        [NotNull]        
        protected static string GetCallingClassName(int skipFrames = 2)
        {
            Type declaringType;
            string str;
            do
            {
                var method = new StackFrame(skipFrames, false).GetMethod();
                declaringType = method.DeclaringType;
                if (declaringType == null)
                {
                    str = method.Name;
                    break;
                }
                ++skipFrames;
                str = declaringType.Name;
            }
            while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));
            return str;
        }

        private readonly string _fixtureName;

        #endregion
    }


    internal static class TestBoundary
    {
        /// <summary> Шаблон пограничного readonly-метода с параметрами, c возвращаемым значением.</summary>
        public static void Execute<TParams>(string typeName, string methodName, TParams parameters, [NotNull] Action methodBody)
            => Invoke(typeName, methodName, parameters, methodBody);

        /// <summary> Шаблон пограничного readonly-метода без параметров, с возвращаемым значением.</summary>
        public static void Execute(string typeName, string methodName, [NotNull] Action methodBody)
            => Invoke(typeName, methodName, LogLevel.Debug, methodBody);


        #region Private members

        /// <summary> Шаблон пограничного метода с параметрами, без возвращаемого значения.</summary>
        private static void Invoke<TParams>(string typeName, string methodName, TParams parameters, [NotNull] Action methodBody)
        {
            var stopwatch = Stopwatch.StartNew();
            var mdcContext = false;
            try
            {
                mdcContext = EnterContext(typeName, methodName);
                LogInvoke(methodName, parameters);
                (methodBody ?? throw new ArgumentNullException(nameof(methodBody)))();
            }
            catch (Exception x)
            {
                LogFail(x, stopwatch.ElapsedMilliseconds);
                throw;
            }
            finally
            {
                LeaveContext(mdcContext);
            }
        }


        /// <summary> Шаблон пограничного метода без параметров и без возвращаемого значениея.</summary>
        private static void Invoke(string typeName, string methodName, [NotNull] Action methodBody)
        {
            var stopwatch = Stopwatch.StartNew();
            var mdcContext = false;
            try
            {
                mdcContext = EnterContext(typeName, methodName);
                LogInvoke(methodName);
                (methodBody ?? throw new ArgumentNullException(nameof(methodBody)))();
            }
            catch (Exception x)
            {
                LogFail(x, stopwatch.ElapsedMilliseconds);
                throw;
            }
            finally
            {
                LeaveContext(mdcContext);
            }
        }


        private static bool EnterContext(string typeName, string methodName)
        {
            if (!string.IsNullOrEmpty(MappedDiagnosticsContext.Get("SERVICE")))
            {
                _logger.Warn("Second enter to MDC context ignored.");
                return false;
            }

            MappedDiagnosticsContext.Set("FIXTURE", typeName);
            MappedDiagnosticsContext.Set("TEST", methodName);
            return true;
        }


        private static void LeaveContext(bool mdcContext)
        {
            if (mdcContext)
            {
                MappedDiagnosticsContext.Remove("TEST");
                MappedDiagnosticsContext.Remove("FIXTURE");
            }
            LogManager.Flush();
        }


        private static void LogInvoke(string methodName)
            => _logger.Info($"MethodInvoke: {{ Method: {methodName} }}");


        private static void LogInvoke<TParameters>(string methodName, TParameters parameters)
            => _logger.Info($"MethodInvoke: {{ Method: {methodName}, Parameters: {parameters} }}");


        private static void LogFail(Exception x, long elapsedMilliseconds)
            => _logger.Error($"MethodFail: {{ ElapsedMilliseconds: {elapsedMilliseconds:F}, Exception: {Environment.NewLine}{x} }}");


        private static void LogReturn(long elapsedMilliseconds)
            => _logger.Info($"MethodReturn: {{ ElapsedMilliseconds: {elapsedMilliseconds:F} }}");


        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        #endregion
    }
}