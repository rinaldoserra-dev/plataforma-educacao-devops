using EasyNetQ;
using PlataformaEducacao.Core.Messages.Integration;
using Polly;
using RabbitMQ.Client.Exceptions;

namespace PlataformaEducacao.MessageBus
{
    public class MessageBus : IMessageBus
    {
        private readonly string _connectionString;
        private IBus _bus = null!;
        private IAdvancedBus _advancedBus = null!;

        public MessageBus(string connectionString)
        {
            _connectionString = connectionString;
            TryConnect();
        }

        public bool IsConnected => _bus?.IsConnected ?? false;

        public IAdvancedBus AdvancedBus => _bus?.Advanced!;

        public void Publish<T>(T message)
            where T : IntegrationEvent
        {
            TryConnect();
            GetBus().Publish(message);
        }

        public async Task PublishAsync<T>(T message)
            where T : IntegrationEvent
        {
            TryConnect();
            await GetBus().PublishAsync(message);
        }

        public void Subscribe<T>(string subscriptionId, Action<T> onMessage)
            where T : class
        {
            TryConnect();
            GetBus().Subscribe(subscriptionId, onMessage);
        }

        public void SubscribeAsync<T>(string subscriptionId, Func<T, Task> onMessage)
            where T : class
        {
            TryConnect();
            GetBus().SubscribeAsync(subscriptionId, onMessage);
        }

        public TResponse Request<TRequest, TResponse>(TRequest request)
            where TRequest : IntegrationEvent
            where TResponse : ResponseMessage
        {
            TryConnect();
            return GetBus().Request<TRequest, TResponse>(request);
        }

        public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request)
            where TRequest : IntegrationEvent
            where TResponse : ResponseMessage
        {
            TryConnect();
            return await GetBus().RequestAsync<TRequest, TResponse>(request);
        }

        public IDisposable Respond<TRequest, TResponse>(Func<TRequest, TResponse> responder)
            where TRequest : IntegrationEvent
            where TResponse : ResponseMessage
        {
            TryConnect();
            return GetBus().Respond(responder);
        }

        public IDisposable RespondAsync<TRequest, TResponse>(Func<TRequest, Task<TResponse>> responder)
            where TRequest : IntegrationEvent
            where TResponse : ResponseMessage
        {
            TryConnect();
            return GetBus().RespondAsync(responder);
        }

        public void Dispose()
        {
            _bus.Dispose();
        }

        private void TryConnect()
        {
            if (IsConnected) return;

            var policy = Policy.Handle<EasyNetQException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            try
            {
                policy.Execute(() =>
                {
                    _bus = RabbitHutch.CreateBus(_connectionString);
                    _advancedBus = _bus.Advanced;
                    _advancedBus.Disconnected += OnDisconnect;
                });
            }
            catch (Exception) when (_bus is null || IsConnected is false)
            {
                // Readiness reports the unavailable broker while the API remains startable.
            }
        }

        private IBus GetBus()
        {
            if (_bus is null || IsConnected is false)
                throw new EasyNetQException("RabbitMQ is not connected.");

            return _bus;
        }

        private void OnDisconnect(object? s, EventArgs e)
        {
            var policy = Policy.Handle<EasyNetQException>()
                .Or<BrokerUnreachableException>()
                .RetryForever();

            policy.Execute(TryConnect);
        }
    }
}
