using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using NLog;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure.Testing
{
    /// <inheritdoc />
    /// <summary> Управляет временем жизни оконного приложения в стиле IDisposable. </summary>
    /// 
    public sealed class WinAppController : IDisposable
    {
        /// <inheritdoc />
        /// <summary> Инициирует старт приложения и дожидается, 
        /// когда приложение перейдёт в состояние ожидания.</summary>
        public WinAppController([NotNull] string exeFilePath)
            : this(new WinAppStartInfo { ExePath = exeFilePath })
        {
        }


        /// <summary> Инициирует старт приложения и дожидается, 
        /// когда приложение перейдёт в состояние ожидания.</summary>
        public WinAppController([NotNull] WinAppStartInfo appInfo)
        {
            _logger.Trace($"Start application: {appInfo}. ");

            if (string.IsNullOrWhiteSpace(appInfo.ExePath))
                throw new InvalidOperationException("Path to executable file has not been specified.");

            if (!Path.IsPathRooted(appInfo.ExePath))
                appInfo.ExePath = TestEnvironment.GetFilePath(appInfo.ExePath);

            _process = Process.Start(new ProcessStartInfo
            {
                FileName         = appInfo.ExePath,
                Arguments        = appInfo.Arguments,
                WorkingDirectory = appInfo.WorkingDirectory
            });
            if (_process == null)
                throw new InvalidOperationException($"Can't start application '{appInfo.ExePath}'.");

            _process.WaitForInputIdle(10000);
            var startDate = DateTime.Now;
            while (!_process.Responding)
            {
                Thread.Sleep(100);
                if ((DateTime.Now -startDate).TotalSeconds > 10000)
                    throw new InvalidOperationException($"Can't start application '{appInfo.ExePath}'.");
            }
            _logger.Info($"Application '{appInfo.ExePath}' started");
        }


        /// <inheritdoc />
        /// <summary> Завершает приложение закрывая главное окно приложения. </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            if (!_process.CloseMainWindow())
            {
                _logger.Error("Can't clase main window of application. Killing application...");
                _process.Kill();
            }

            if (!_process.WaitForExit(10000))
            {
                _logger.Error("Can't stop application. Killing application...");
                _process.Kill();
            }
            _disposed = true;
            _logger.Info($"Application '{_process.StartInfo.FileName}' stopped.");
        }


        #region Private members

        private bool _disposed;
        private readonly Process _process;
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        
        #endregion Private members
    }
}
