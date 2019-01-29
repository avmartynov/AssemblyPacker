using System;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure
{
    public sealed class AppDomainIsolated<T> : IDisposable where T : MarshalByRefObject
    {
        public AppDomainIsolated([CanBeNull] UnhandledExceptionEventHandler unhandledExceptionEventHandler = null)
        {
            _domain = AppDomain.CreateDomain(friendlyName: Guid.NewGuid().ToString(), 
                                             securityInfo: null, 
                                                     info: AppDomain.CurrentDomain.SetupInformation);

            var type = typeof(T);
            if (type.FullName == null)
                throw new InvalidOperationException("Invalid type for domain isolated call.");

            if (unhandledExceptionEventHandler != null)
                _domain.UnhandledException += unhandledExceptionEventHandler;

            Proxy = (T)_domain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);
        }

        public T Proxy { get; }
 
        public void Dispose()
        {
            if (_domain == null)
                return;

            AppDomain.Unload(_domain);
            _domain = null;
        }


        private AppDomain _domain;
    }
}
