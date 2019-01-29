using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure.Testing
{
    public static class ConsoleAppControl
    {
        public static void StartWaitFinish([NotNull] params string[] paths)
            => StartWaitFinish(new ConsoleAppStartInfo { ExePath = Path.Combine(paths) });


        public static void StartWaitFinish([NotNull] ConsoleAppStartInfo startInfo)
            => Start(startInfo).Dispose();


        [NotNull]
        public static IDisposable Start([NotNull] ConsoleAppStartInfo startInfo)
            => new ConsoleAppController(startInfo);


        public static void StartWaitFinishConfigured([NotNull] string configSectionName)
            => StartConfigured(configSectionName).Dispose();


        [NotNull]
        public static IDisposable StartConfigured([NotNull] string configSectionName)
            => new ConsoleAppController(ReadStartInfoConfigSection(configSectionName));


        [NotNull]
        public static string Execute(string appPath, string args, 
            [CanBeNull] string input = null, [CanBeNull] string workingDirectory = null)
        {
            var startInfo = new ProcessStartInfo
                            {
                                FileName = appPath,
                                WorkingDirectory = workingDirectory ?? "",
                                Arguments = args,
                                RedirectStandardInput  = true,
                                RedirectStandardOutput = true,
                                UseShellExecute        = false,
                            };
            using (var process = Process.Start(startInfo) ?? throw new InvalidOperationException($"Can't start application '{appPath}'."))
            {
                process.StandardInput.Write(input);
                process.StandardInput.Close();
                return process.StandardOutput.ReadToEnd();
            }
        }


        #region Private members

        /// <summary> Читает настройки из конфигурационной секции типа System.Configuration.NameValueFileSectionHandler. </summary>
        [NotNull]
        private static ConsoleAppStartInfo ReadStartInfoConfigSection([NotNull] string configSectionName)
        {
            var config = (NameValueCollection)ConfigurationManager.GetSection(configSectionName);
            if (config == null)
                throw new InvalidOperationException($"Config section '{configSectionName}' does not exist.");

            var si = new ConsoleAppStartInfo
                     {
                         ExePath           = config["ExePath"],
                         WorkingDirectory  = config["WorkingDirectory"],
                         Arguments         = config["Arguments"],
                         WaitLine          = config["WaitLine"],
                         QuitLine          = config["QuitLine"],
                         NoWait = bool.Parse(config["NoWait"] ?? "false")
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
