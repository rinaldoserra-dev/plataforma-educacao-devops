using EasyNetQ;
using FluentValidation.Results;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using PlataformaEducacao.Core.Messages.Integration;
using PlataformaEducacao.GestaoIdentidade.Api.Data;
using PlataformaEducacao.MessageBus;
using static PlataformaEducacao.GestaoIdentidade.Api.Configurations.DbMigrationHelperExtension;

namespace PlataformaEducacao.GestaoIdentidade.Api.Tests.Config
{
    internal sealed class FakeMessageBus : IMessageBus
    {
        public void Dispose()
        {
        }

        public bool IsConnected => true;

        public IAdvancedBus AdvancedBus => throw new NotImplementedException();

        public void Publish<T>(T message)
            where T : IntegrationEvent
        {
        }

        public Task PublishAsync<T>(T message)
            where T : IntegrationEvent
            => Task.CompletedTask;

        public TResponse Request<TRequest, TResponse>(TRequest request)
            where TRequest : IntegrationEvent
            where TResponse : ResponseMessage
            => (TResponse)new ResponseMessage(new ValidationResult());

        public Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request)
            where TRequest : IntegrationEvent
            where TResponse : ResponseMessage
            => Task.FromResult((TResponse)new ResponseMessage(new ValidationResult()));

        public IDisposable Respond<TRequest, TResponse>(Func<TRequest, TResponse> responder)
            where TRequest : IntegrationEvent
            where TResponse : ResponseMessage
            => new FakeDisposable();

        public IDisposable RespondAsync<TRequest, TResponse>(Func<TRequest, Task<TResponse>> responder)
            where TRequest : IntegrationEvent
            where TResponse : ResponseMessage
            => new FakeDisposable();

        public void Subscribe<T>(string subscriptionId, Action<T> onMessage)
            where T : class
        {
        }

        public void SubscribeAsync<T>(string subscriptionId, Func<T, Task> onMessage)
            where T : class
        {
        }

        private sealed class FakeDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}
