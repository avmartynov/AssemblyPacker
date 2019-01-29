using System;
using System.Diagnostics;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace Twidlle.Infrastructure.Wcf
{
    public class WcfServiceHostFactory : ServiceHostFactory
    {
        public WcfServiceHostFactory(Func<Type, object> factory)
            => _factory = factory;


        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            _logger.Info($"ServiceHost base addresses: '{baseAddresses[0]}', ...");
            return new ServiceHost(_factory(serviceType), baseAddresses);
        }


        private readonly Func<Type, object> _factory;

        private static readonly TraceSource _logger = MethodBase.GetCurrentMethod().GetTraceSource();
    }

}
