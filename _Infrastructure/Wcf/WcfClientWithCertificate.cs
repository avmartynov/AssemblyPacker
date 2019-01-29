using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure.Wcf
{
    public sealed class WcfClientWithCertificate<TServiceInterface> : WcfClient<TServiceInterface>
    {
        public WcfClientWithCertificate([NotNull] string endPointName, X509Certificate2 certificate)
            : base(endPointName)
        {
            _certificate = certificate; 
            _channelFactory = CreateFactory(endPointName, certificate);
        }


        protected override ChannelFactory<TServiceInterface> CreateFactory()
            => CreateFactory(_endPointName, _certificate);


        #region Private members

        [NotNull]
        private static ChannelFactory<TServiceInterface> CreateFactory([NotNull] string endPointName, X509Certificate2 certificate)
        {
            var channelFactory = new ChannelFactory<TServiceInterface>(endPointName);

            var clientCredentials = new ClientCredentials();
            clientCredentials.ClientCertificate.Certificate = certificate;

            channelFactory.Endpoint.Behaviors.RemoveAll<ClientCredentials>();
            channelFactory.Endpoint.Behaviors.Add(clientCredentials);

            return channelFactory;
        }

        private readonly X509Certificate2 _certificate;

        #endregion
    }
}
