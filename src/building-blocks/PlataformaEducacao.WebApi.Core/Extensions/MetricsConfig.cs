using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;

namespace PlataformaEducacao.WebApi.Core.Extensions
{
    public static class MetricsConfig
    {
        private static readonly Counter ApplicationExceptions = Metrics.CreateCounter(
            "application_exceptions_total",
            "Total de excecoes nao tratadas observadas pela aplicacao.");

        public static IServiceCollection AddMetricsConfiguration(this IServiceCollection services)
        {
            return services;
        }

        public static IApplicationBuilder UseMetricsConfiguration(this IApplicationBuilder app)
        {
            app.UseHttpMetrics();
            app.Use(async (context, next) =>
            {
                try
                {
                    await next(context);
                }
                catch
                {
                    ApplicationExceptions.Inc();
                    throw;
                }
            });
            app.UseMetricServer();
            return app;
        }
    }
}
