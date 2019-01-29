using System;
using System.ServiceProcess;
using NLog;

namespace Twidlle.Infrastructure.WindowsService
{
    internal sealed class WindowsServiceComponent : ServiceBase
    {
        public WindowsServiceComponent(Func<IDisposable> startupObjectFactory)
        {
            _startupObjectFactory = startupObjectFactory;

            CanStop             = true;
            CanShutdown         = false;
            CanPauseAndContinue = false;
        }


        /// <summary> Используется для старта сервиса как консольного приложения. </summary>
        public void Start()
        {
            OnStart(new string[0]);
        }

        #region Private members

        protected override void OnStart(string[] args)
        {
            try
            {
                if (_startupObject != null)
                {
                    _logger.Warn("Windows service already started.");
                    return;
                }

                _logger.Info("Windows service starting...");
                _logger.Info($"Windows service сonfiguration: {WindowsServiceProcess.ServiceConfig}.");

                _startupObject = _startupObjectFactory();
                    
                _logger.Info("Windows service started.");
            }
            catch (Exception x)
            {
                _logger.Error(x, "Windows service start failed.");
                throw;
            }
        }


        protected override void OnStop()
        {
            try
            {
                if (_startupObject == null)
                    return;

                _logger.Info("Windows service stopping...");

                _startupObject.Dispose();

                _startupObject = null;

                _logger.Info("Windows service stoped.");
            }
            catch (Exception x)
            {
                _logger.Error(x, "Windows service stop failed.");
                throw;
            }
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            OnStop();
        }


        private IDisposable _startupObject;

        private readonly Func<IDisposable> _startupObjectFactory;

        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
         
        #endregion Private members
    }
}