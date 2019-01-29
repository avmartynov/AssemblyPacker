using System;
using System.ServiceModel;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure.Wcf
{
    public class WcfClient<TServiceInterface> : IDisposable
    {
        public WcfClient([NotNull] string endPointName) 
        {
            _endPointName = endPointName;
            _channelFactory = CreateFactory(endPointName);
        }


        public TResult Call<TResult>([NotNull] Func<TServiceInterface, TResult> action)
        {
            if (_channelFactory == null)
                throw new ObjectDisposedException(nameof(WcfClient<TServiceInterface>));

            if (_channelFactory.State == CommunicationState.Faulted ||
                _channelFactory.State == CommunicationState.Closed)
            {
                ((IDisposable)_channelFactory).Dispose();
                _channelFactory = CreateFactory();
            }

            var proxy = _channelFactory.CreateChannel();
            var channel = (IClientChannel) proxy;
            try
            {
                channel.Open();
                var result = action(proxy);
                channel.Close();
                return result;
            }
            catch (Exception)
            {
                channel.Abort();
                throw;
            }
        }


        public void Call([NotNull] Action<TServiceInterface> action)
        {
            if (_channelFactory == null)
                throw new ObjectDisposedException(nameof(WcfClient<TServiceInterface>));

            if (_channelFactory.State == CommunicationState.Faulted ||
                _channelFactory.State == CommunicationState.Closed)
            {
                ((IDisposable)_channelFactory).Dispose();
                _channelFactory = CreateFactory();
            }

            var proxy = _channelFactory.CreateChannel();
            var channel = (IClientChannel) proxy;
            try
            {
                channel.Open();
                action(proxy);
                channel.Close();
            }
            catch (Exception)
            {
                channel.Abort();
                throw;
            }
        }


        public void Dispose()
        {
            Dispose(true);  
            GC.SuppressFinalize(this);  
        }


        #region Protected members

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _channelFactory == null)
                return;

            ((IDisposable)_channelFactory).Dispose();
            _channelFactory = null;
        }



        [NotNull]
        protected virtual ChannelFactory<TServiceInterface> CreateFactory() 
            => CreateFactory(_endPointName);

        #endregion
        #region Private members

        ~WcfClient()
        {
            Dispose(false);  
        }

        [NotNull]
        private static ChannelFactory<TServiceInterface> CreateFactory([NotNull] string endPointName) 
            => new ChannelFactory<TServiceInterface>(endPointName);

        protected ChannelFactory<TServiceInterface> _channelFactory;
        protected readonly string _endPointName;

        #endregion
    }
}
