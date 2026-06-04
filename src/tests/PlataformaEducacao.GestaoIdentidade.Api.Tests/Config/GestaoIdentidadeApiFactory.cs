using EasyNetQ;
using FluentValidation.Results;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using PlataformaEducacao.Core.Messages.Integration;
using PlataformaEducacao.GestaoIdentidade.Api.Data;
using PlataformaEducacao.MessageBus;
using static PlataformaEducacao.GestaoIdentidade.Api.Configurations.DbMigrationHelperExtension;

namespace PlataformaEducacao.GestaoIdentidade.Api.Tests.Config
{
    public class GestaoIdentidadeApiFactory : WebApplicationFactory<Program>, IDisposable
    {
        private SqliteConnection _connection = null!;

        public GestaoIdentidadeApiFactory()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContextTypes = new[]
                {
                    typeof(GestaoIdentidadeContext)
                };

                foreach (var dbContextType in dbContextTypes)
                {
                    var descriptorsToRemove = services
                        .Where(d =>
                            d.ServiceType == dbContextType ||
                            (d.ServiceType.IsGenericType &&
                             d.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>) &&
                             d.ServiceType.GenericTypeArguments[0] == dbContextType) ||
                            d.ServiceType == typeof(DbContextOptions))
                        .ToList();

                    foreach (var descriptor in descriptorsToRemove)
                    {
                        services.Remove(descriptor);
                    }
                }

                services.AddDbContext<GestaoIdentidadeContext>(options => options.UseSqlite(_connection));

                var hostedServices = services.Where(
                    d => d.ServiceType == typeof(IHostedService)).ToList();
                foreach (var hs in hostedServices)
                    services.Remove(hs);

                services.RemoveAll<IMessageBus>();
                services.AddSingleton<IMessageBus>(new FakeMessageBus());

                using (var scope = services.BuildServiceProvider().CreateScope())
                {
                    var serviceProvider = scope.ServiceProvider;
                    DbMigrationHelper.EnsureSeedData(serviceProvider).GetAwaiter().GetResult();
                }
            });
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            return base.CreateHost(builder);
        }

        public new void Dispose()
        {
            base.Dispose();

            if (_connection != null)
            {
                _connection.Close();
            }
        }
    }

    internal class FakeMessageBus : IMessageBus
    {
        public void Dispose() { }

        public bool IsConnected => true;

        public IAdvancedBus AdvancedBus => throw new NotImplementedException();

        public void Publish<T>(T message) where T : IntegrationEvent { }

        public Task PublishAsync<T>(T message) where T : IntegrationEvent
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

        public void Subscribe<T>(string subscriptionId, Action<T> onMessage) where T : class { }

        public void SubscribeAsync<T>(string subscriptionId, Func<T, Task> onMessage) where T : class { }

        private class FakeDisposable : IDisposable
        {
            public void Dispose() { }
        }
    }
}
