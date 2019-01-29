using System.ServiceModel;
using System.ServiceModel.Description;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure.Wcf
{
    public sealed class WcfClientWithUserNamePassword<TServiceInterface> : WcfClient<TServiceInterface> 
    {
        public WcfClientWithUserNamePassword([NotNull] string endPointName, string userName, string password)
            : base(endPointName)
        {
            _userName = userName;
            _password = password;
            _channelFactory = CreateFactory(endPointName, userName, password);
        }


        /// <inheritdoc />
        protected override ChannelFactory<TServiceInterface> CreateFactory()
            => CreateFactory(_endPointName, _userName, _password);


        #region Private members

        [NotNull] 
        private static ChannelFactory<TServiceInterface> CreateFactory([NotNull] string endPointName, string userName, string password) 
        {
            var channelFactory = new ChannelFactory<TServiceInterface>(endPointName);

            var clientCredentials = new ClientCredentials();
            clientCredentials.UserName.UserName = userName;
            clientCredentials.UserName.Password = password;

            channelFactory.Endpoint.Behaviors.RemoveAll<ClientCredentials>();
            channelFactory.Endpoint.Behaviors.Add(clientCredentials);

            return channelFactory;
        }

        private readonly string _userName;
        private readonly string _password;

        #endregion
    }
}
