using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Threading;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure.Testing
{
    /// <summary> Управление временем жизни оконного приложения в стиле IDisposable. </summary>
    public static class WinAppControl
    {
        /// <summary> Инициирует старт приложения и дожидается, 
        /// когда приложение перейдёт в состояние ожидания.
        /// Читает настройки запуска приложения из секции типа 
        /// System.Configuration.NameValueFileSectionHandler. </summary>
        [NotNull] 
        public static IDisposable StartConfigured([NotNull] string configSectionName = "applicationUnderTest")
        {
            var config = ReadStartInfoConfigSection(configSectionName);
            return new WinAppController(config);
        }


        /// <summary> Инициирует старт приложения и дожидается, 
        /// когда приложение перейдёт в состояние ожидания.
        /// Читает настройки запуска приложения из секции типа 
        /// System.Configuration.NameValueFileSectionHandler. </summary>
        [NotNull] 
        public static IDisposable Start([NotNull] string exeFilePath)
            => new WinAppController(new WinAppStartInfo { ExePath = exeFilePath});


        /// <summary> Инициирует старт приложения и дожидается, 
        /// когда приложение перейдёт в состояние ожидания, потом ещё ждёт waitSeconds секунд.
        /// Читает настройки запуска приложения из секции типа 
        /// System.Configuration.NameValueFileSectionHandler. </summary>
        public static void StartWaitFinishConfigured([NotNull] string configSectionName = "applicationUnderTest", int waitSeconds = 0)
        {
            using (StartConfigured(configSectionName))
                Thread.Sleep(TimeSpan.FromSeconds(waitSeconds)); 
        }


        public static void StartWaitFinish(int waitSeconds, [NotNull] params string[] paths)
        {
            using (Start(Path.Combine(paths)))
                Thread.Sleep(TimeSpan.FromSeconds(waitSeconds)); 
        }


        #region Private members

        /// <summary> Читает настройки из конфигурационной секции
        /// типа System.Configuration.NameValueFileSectionHandler. </summary>
        [NotNull]
        private static WinAppStartInfo ReadStartInfoConfigSection([NotNull] string configSectionName)
        {
            var section = ConfigurationManager.GetSection(configSectionName) ??
                          throw new InvalidOperationException($"Config section '{configSectionName}' does not exist.");

            var config = (section as NameValueCollection) ??
                         throw new InvalidOperationException($"Config section '{configSectionName}' must be NameValueFileSectionHandler.");

            var si = new WinAppStartInfo
                     {
                         ExePath           = config["ExePath"],
                         WorkingDirectory  = config["WorkingDirectory"],
                         Arguments         = config["Arguments"],
                     };

            if (string.IsNullOrWhiteSpace(si.ExePath))
                throw new InvalidOperationException("Path to executable file has not been specified.");

            if (!Path.IsPathRooted(si.ExePath))
                si.ExePath = Path.Combine(TestEnvironment.GetTestDirectory(), si.ExePath).PathCanonicalize();

            if (string.IsNullOrEmpty(si.WorkingDirectory))
                si.WorkingDirectory = TestEnvironment.GetTestDirectory();

            else if (!Path.IsPathRooted(si.WorkingDirectory))
                si.WorkingDirectory = Path.Combine(TestEnvironment.GetTestDirectory(), si.WorkingDirectory).PathCanonicalize();

            si.Arguments = si.Arguments?.Replace("${{WorkingDirectory}}", si.WorkingDirectory);
            return si;
        }

        #endregion Private members
    }
}
