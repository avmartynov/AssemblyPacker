using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Collections;
using System.Security.Permissions;
using NLog;
using Newtonsoft.Json;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure.Diagnostics
{
    /// <summary> Шаблонные методы для использования на границах слоёв, 
    /// в частности, на внешней границе слоя бизнес-логики. </summary>
    /// <remark>
    /// Шаблонные методы обеспечивают дополнительные аспекты диагностики и безопасности 
    /// работы сервиса, а именно:
    /// 1. Логирование факта входа/выхода в метод;
    /// 2. Контроль прав на выполнение метода (требование разрешений роли);
    /// 3. NLog-контекст для всех внутренних вызовов (NLog.MappedDiagnosticsContext);
    /// 4. Логирование входных параметров и возвращаемого значения метода;
    /// 5. Логирование возникающих в методе исключений.
    /// 6. Измерение и логирование производительности (времени выполнения).
    /// 
    /// Подробности:
    /// - Исключения только логируются (не глушатся и не перевысвечиваются);
    /// - NLog-контекст состоит из двух параметров: SERVICE, METHOD;
    /// - Логирование readonly-методов делается на уровне логирования Debug;
    /// - Логирование side-effects-методов делается на уровне логирования Info;
    /// - Большие возвращаемые значения (перечисления) логируются "лениво" и только 
    ///     при включённом максимальном уровне логирования (Trace) с помощью Newtonsoft.Json.
    /// </remark>
    public class LayerFacade
    {
        public LayerFacade(string serviceName)
            => _serviceName = serviceName;

        [MethodImpl(MethodImplOptions.NoInlining)]
        [NotNull]
        public static LayerFacade GetCurrentClassFacade()
            => new LayerFacade(GetCallingClassName());

        public TResult Request<TResult, TParams>(TParams parameters, Func<TResult> methodBody, [CanBeNull] string requiredRole = null, [CanBeNull] [CallerMemberName] string methodName = null)
            => LayerBoundary.Request(_serviceName, methodName, requiredRole, parameters, () => Invoke(methodBody));

        public TResult Request<TResult>(Func<TResult> methodBody, [CanBeNull] string requiredRole = null, [CanBeNull] [CallerMemberName] string methodName = null)
            => LayerBoundary.Request(_serviceName, methodName, requiredRole, () => Invoke(methodBody));

        public TResult Command<TResult, TParams>(TParams parameters, Func<TResult> methodBody, [CanBeNull] string requiredRole = null, [CanBeNull] [CallerMemberName] string methodName = null)
            => LayerBoundary.Command(_serviceName, methodName, requiredRole, parameters, () => Invoke(methodBody));

        public TResult Command<TResult>(Func<TResult> methodBody, [CanBeNull] string requiredRole = null, [CanBeNull] [CallerMemberName] string methodName = null)
            => LayerBoundary.Command(_serviceName, methodName, requiredRole, () => Invoke(methodBody));

        public void Command<TParams>(TParams parameters, Action methodBody, [CanBeNull] string requiredRole = null, [CanBeNull] [CallerMemberName] string methodName = null)
            => LayerBoundary.Command(_serviceName, methodName, requiredRole, parameters, () => Invoke(methodBody));

        public void Command(Action methodBody, [CanBeNull] string requiredRole = null, [CanBeNull] [CallerMemberName] string methodName = null)
            => LayerBoundary.Command(_serviceName, methodName, requiredRole, () => Invoke(methodBody));

        protected virtual TResult Invoke<TResult>([NotNull] Func<TResult> methodBody) 
            => (methodBody?? throw new ArgumentNullException(nameof(methodBody)))();

        protected virtual void Invoke([NotNull] Action methodBody) 
            => (methodBody ?? throw new ArgumentNullException(nameof(methodBody)))();

        #region Private members

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

        private readonly string _serviceName;

        #endregion
    }


    internal static class LayerBoundary
    {
        /// <summary> Шаблон пограничного readonly-метода с параметрами, c возвращаемым значением.</summary>
        public static TResult Request<TResult, TParams>(string typeName, string methodName, string requiredRole, TParams parameters, Func<TResult> methodBody)
            => Invoke(typeName, methodName, LogLevel.Debug, requiredRole, parameters, methodBody);

        /// <summary> Шаблон пограничного readonly-метода без параметров, с возвращаемым значением.</summary>
        public static TResult Request<TResult>(string typeName, string methodName, string requiredRole, Func<TResult> methodBody)
            => Invoke(typeName, methodName, LogLevel.Debug, requiredRole, methodBody);

        /// <summary> Шаблон пограничного side-effects-метода с параметрами, c возвращаемым значением.</summary>
        public static TResult Command<TResult, TParams>(string typeName, string methodName, string requiredRole, TParams parameters, Func<TResult> methodBody)
            => Invoke(typeName, methodName, LogLevel.Info, requiredRole, parameters, methodBody);

        /// <summary> Шаблон пограничного side-effects метода без параметров, с возвращаемым значением.</summary>
        public static TResult Command<TResult>(string typeName, string methodName, string requiredRole, Func<TResult> methodBody)
            => Invoke(typeName, methodName, LogLevel.Info, requiredRole, methodBody);

        /// <summary> Шаблон пограничного side-effects метода с параметрами, без возвращаемого значения.</summary>
        public static void Command<TParams>(string typeName, string methodName, string requiredRole, TParams parameters, [NotNull] Action methodBody)
            => Invoke(typeName, methodName, LogLevel.Info, requiredRole, parameters, methodBody);

        /// <summary> Шаблон пограничного side-effects метода без параметров и без возвращаемого значениея.</summary>
        public static void Command(string typeName, string methodName, string requiredRole, [NotNull] Action methodBody)
            => Invoke(typeName, methodName, LogLevel.Info, requiredRole, methodBody);

        #region Private members

        /// <summary> Шаблон пограничного метода с параметрами, c возвращаемым значением.</summary>
        private static TResult Invoke<TResult, TParams>(string typeName, string methodName, 
            LogLevel logLevel, string  requiredRole, TParams parameters, Func<TResult> methodBody)
        {
            var stopwatch = Stopwatch.StartNew();
            var mdcContext = false;
            try
            {
                mdcContext = EnterContext(typeName, methodName);
                LogInvoke(logLevel, methodName, parameters);
                DemandPermissionsForRole(requiredRole);
                var result = (methodBody ?? throw new ArgumentNullException(nameof(methodBody)))();
                LogResult(logLevel, result, stopwatch.ElapsedMilliseconds);
                return result;
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


        /// <summary> Шаблон пограничного метода без параметров, с возвращаемым значением.</summary>
        private static TResult Invoke<TResult>(string typeName, string methodName, LogLevel logLevel, string requiredRole, Func<TResult> methodBody)
        {
            var stopwatch = Stopwatch.StartNew();
            var mdcContext = false;
            try
            {
                mdcContext = EnterContext(typeName, methodName);
                LogInvoke(logLevel, methodName);
                DemandPermissionsForRole(requiredRole);
                var result = (methodBody ?? throw new ArgumentNullException(nameof(methodBody)))();
                LogResult(logLevel, result, stopwatch.ElapsedMilliseconds);
                return result;
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


        /// <summary> Шаблон пограничного метода с параметрами, без возвращаемого значения.</summary>
        private static void Invoke<TParams>(string typeName, string methodName, LogLevel logLevel, string  requiredRole, TParams parameters, [NotNull] Action methodBody)
        {
            var stopwatch = Stopwatch.StartNew();
            var mdcContext = false;
            try
            {
                mdcContext = EnterContext(typeName, methodName);
                LogInvoke(logLevel, methodName, parameters);
                DemandPermissionsForRole(requiredRole);
                (methodBody ?? throw new ArgumentNullException(nameof(methodBody)))();
                LogReturn(logLevel, stopwatch.ElapsedMilliseconds);
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
        private static void Invoke(string typeName, string methodName, LogLevel logLevel, string requiredRole, [NotNull] Action methodBody)
        {
            var stopwatch = Stopwatch.StartNew();
            var mdcContext = false;
            try
            {
                mdcContext = EnterContext(typeName, methodName);
                LogInvoke(logLevel, methodName);
                DemandPermissionsForRole(requiredRole);
                (methodBody ?? throw new ArgumentNullException(nameof(methodBody)))();
                LogReturn(logLevel, stopwatch.ElapsedMilliseconds);
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

            MappedDiagnosticsContext.Set("SERVICE", typeName);
            MappedDiagnosticsContext.Set("METHOD", methodName);
            return true;
        }


        private static void LeaveContext(bool mdcContext)
        {
            if (mdcContext)
            {
                MappedDiagnosticsContext.Remove("METHOD");
                MappedDiagnosticsContext.Remove("SERVICE");
            }
            LogManager.Flush();
        }


        private static void LogInvoke(LogLevel logLevel, string methodName)
        {
            _logger.Log(logLevel, $"MethodInvoke: {{ Method: {methodName} }}");
        }


        private static void LogInvoke<TParameters>(LogLevel logLevel, string methodName, TParameters parameters)
        {
            _logger.Log(logLevel, $"MethodInvoke: {{ Method: {methodName}, Parameters: {parameters} }}");
        }


        private static void LogFail(Exception x, long elapsedMilliseconds)
        {
            _logger.Error($"MethodFail: {{ ElapsedMilliseconds: {elapsedMilliseconds:F}, Exception: {Environment.NewLine}{x} }}");
        }


        private static void LogReturn(LogLevel logLevel, long elapsedMilliseconds)
        {
            _logger.Log(logLevel, $"MethodReturn: {{ ElapsedMilliseconds: {elapsedMilliseconds:F} }}");
        }


        private static void LogResult<TResult>(LogLevel logLevel, TResult result, long elapsedMilliseconds)
        {
            if (result is IEnumerable enumerable)
            {
                _logger.Trace(() => $"Result: {JsonConvert.SerializeObject(enumerable)}");
                _logger.Log(logLevel, $"MethodReturn: {{ ElapsedMilliseconds: {elapsedMilliseconds:F}, "
                                    + $"Items = {enumerable.Cast<object>().Count()} }}");
            }
            else
            {
                _logger.Log(logLevel, $"MethodReturn: {{ ElapsedMilliseconds: {elapsedMilliseconds:F}, " 
                                    + $"Result: {result} }}");
            }
        }


        private static void DemandPermissionsForRole([CanBeNull] string requiredRole)
        {
            if (requiredRole == null)
                return;

            var permissions = new PrincipalPermission(null, requiredRole);
            permissions.Demand();
        }

        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        #endregion
    }
}