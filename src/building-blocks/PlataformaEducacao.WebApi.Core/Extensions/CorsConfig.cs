using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PlataformaEducacao.WebApi.Core.Extensions
{
    public static class CorsConfig
    {
        public static IServiceCollection AddCorsConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var allowedOrigins = configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>()
                ?? Array.Empty<string>();

            services.AddCors(options =>
            {
                options.AddPolicy("Frontend", policy =>
                {
                    if (allowedOrigins.Length > 0)
                    {
                        policy.WithOrigins(allowedOrigins);
                    }

                    policy
                        .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
                        .WithHeaders("Authorization", "Content-Type", "X-Correlation-ID");
                });
            });

            return services;
        }
    }
}
