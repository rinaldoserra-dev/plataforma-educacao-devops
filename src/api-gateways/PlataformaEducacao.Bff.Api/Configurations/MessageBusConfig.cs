namespace PlataformaEducacao.Bff.Api.Configurations
{
    public static class MessageBusConfig
    {
        public static IServiceCollection AddMessageBusConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }
    }
}
