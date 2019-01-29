using System;
using System.Diagnostics;
using System.IO;
using NLog;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure.Testing
{
    /// <inheritdoc />
    /// <summary> Управляет временем жизни консольного приложения в стиле IDisposable. </summary>
    /// <remarks>
    /// 1. Для корректной работы этого класса, консольное приложение должно завершаться путём нажатия 
    ///     на клавишу [Enter] (т.е. зависать на опереторе Console.ReadLine()).
    /// 2. При создании объекта, приложение запускается, затем (опционально) происходит ожидание его полной готовности к работе.
    /// 3. При удалении объекта имитируется нажатие на [Enter], в результате чего приложение самостоятельно завершается.
    /// </remarks>
    public sealed class ConsoleAppController : IDisposable
    {
        /// <summary> Инициирует старт приложения и дожидается, 
        /// когда приложение перейдёт в состояние ожидания.</summary>
        public ConsoleAppController([NotNull] ConsoleAppStartInfo appInfo)
        {
            _logger.Trace($"Start application: {appInfo}. ");

            if (string.IsNullOrWhiteSpace(appInfo.ExePath))
                throw new InvalidOperationException("Path to executable file has not been specified.");

            if (!Path.IsPathRooted(appInfo.ExePath))
                appInfo.ExePath = TestEnvironment.GetFilePath(appInfo.ExePath);

            appInfo.WaitLine = appInfo.WaitLine ?? "Press ";
            
            _process = Process.Start(new ProcessStartInfo
            {
                FileName         = appInfo.ExePath,
                Arguments        = appInfo.Arguments,
                WorkingDirectory = appInfo.WorkingDirectory,

                RedirectStandardInput  = true,
                RedirectStandardOutput = true,
                UseShellExecute        = false
            });
            if (_process == null)
                throw new InvalidOperationException($"Can't start application '{appInfo.ExePath}'.");

            if (appInfo.NoWait)
                return;

            var allLines = Environment.NewLine;
            while (true)
            {
                var line = _process.StandardOutput.ReadLine();
                if (line == null)
                {
                    _logger.Error($"Аpplication '{appInfo.ExePath}' output: {allLines}");
                    throw new InvalidOperationException($"Аpplication '{appInfo.ExePath}' unexpected finished.");
                }

                allLines += Environment.NewLine + line;
                if (!line.Contains(appInfo.WaitLine))
                    continue;

                _logger.Trace($"Аpplication '{appInfo.ExePath}' output: {allLines}");
                _logger.Info($"Application '{appInfo.ExePath}' started");
                break;
            }
            _quitLine = appInfo.QuitLine;
        }


        /// <inheritdoc />
        /// <summary> Завершает приложение эмитируя нажатие клавиши [Enter]. </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _process.StandardInput.WriteLine(string.IsNullOrEmpty(_quitLine) ? string.Empty : _quitLine);

            if (_quitLine == "KillJustNow")
                _process.Kill();

            if (!_process.WaitForExit(10000))
            {
                _logger.Error("Can't stop application. Killing it...");
                _process.Kill();
            }
            _disposed = true;
            _logger.Info($"Application '{_process.StartInfo.FileName}' stopped.");
        }

        #region Private members

        private bool _disposed;
        private readonly Process _process;
        private readonly string _quitLine;
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        
        #endregion Private members
    }
}
