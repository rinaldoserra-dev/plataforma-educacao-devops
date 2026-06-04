using PlataformaEducacao.Core.Utils;
using PlataformaEducacao.MessageBus;

namespace PlataformaEducacao.GestaoIdentidade.Api.Configurations
{
    public static class MessageBusConfig
    {
        public static IServiceCollection AddMessageBusConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus"));

            return services;
        }
    }
}
