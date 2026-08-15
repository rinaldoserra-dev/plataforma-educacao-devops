using PlataformaEducacao.Core.Utils;
using PlataformaEducacao.MessageBus;

namespace PlataformaEducacao.GestaoConteudo.Api.Configurations
{
    public static class MessageBusConfig
    {
        public static IServiceCollection AddMessageBusConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            return services.AddMessageBus(
                configuration.GetMessageQueueConnection("MessageBus"));
        }
    }
}
