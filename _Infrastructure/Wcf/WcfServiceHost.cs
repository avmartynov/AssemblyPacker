using System;
using System.Diagnostics;
using System.Reflection;
using System.ServiceModel;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure.Wcf
{
    /// <inheritdoc />
    /// <summary>  Управление временем жизни WCF-сервиса в стиле IDisposable. </summary>
    public abstract class WcfServiceHost : IDisposable
    {
        protected WcfServiceHost([NotNull] object wcfService)
        {
            _logger.Info($"WCF service '{wcfService}' opening...");

            _serviceHost = new ServiceHost(wcfService);
            _serviceHost.Open();

            _logger.Info($"WCF service '{wcfService}' opened.");
        }

        public void Dispose()
        {
            Dispose(true);  
            GC.SuppressFinalize(this);  
        }

        #region Protected members

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            if (_disposed)
                return;

            _logger.Info($"WCF service '{_serviceHost.SingletonInstance}' closing...");

            if (_serviceHost.State == CommunicationState.Opening 
             || _serviceHost.State == CommunicationState.Opened)
            {
                try
                {
                    _serviceHost.Close();
                    _logger.Info($"WCF service '{_serviceHost.SingletonInstance}' closed.");
                }
                catch (Exception x)
                {
                    _logger.Error(x, $"WCF service '{_serviceHost.SingletonInstance}' closing fail.");

                    _serviceHost.Abort();
                }
            }
            _disposed = true;
        }

        #endregion
        #region Private members

        ~WcfServiceHost()
        {
            Dispose(false);  
        }

        private bool _disposed;
        private readonly ServiceHost _serviceHost;


        private static readonly TraceSource _logger = MethodBase.GetCurrentMethod().GetTraceSource();

        #endregion
    }


    public sealed class WcfServiceHost<TService> : WcfServiceHost
    {
        public WcfServiceHost([NotNull] TService wcfService) : base(wcfService)
        { }
    }

}
