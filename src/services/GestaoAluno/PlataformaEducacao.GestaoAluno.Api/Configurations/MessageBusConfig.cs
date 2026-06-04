using PlataformaEducacao.Core.Utils;
using PlataformaEducacao.GestaoAluno.Application.Services;
using PlataformaEducacao.MessageBus;

namespace PlataformaEducacao.GestaoAluno.Api.Configurations
{
    public static class MessageBusConfig
    {
        public static IServiceCollection AddMessageBusConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus"))
                .AddHostedService<RegistroAlunoIntegrationHandler>();

            services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus"))
                .AddHostedService<PagamentoMatriculaIntegrationHandler>();

            return services;
        }
    }
}
