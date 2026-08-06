using CorrelationId;
using CorrelationId.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace PlataformaEducacao.WebApi.Core.Extensions;


public static class LoggingConfig
{
    public static IHostBuilder AddLoggingConfiguration(
        this IHostBuilder hostBuilder,
        IConfiguration configuration,
        string serviceName)
    {
        hostBuilder.UseSerilog((context, _, loggerConfig) =>
        {
            loggerConfig
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .Enrich.WithProperty("Service", serviceName)
                .Enrich.WithProperty("Application", serviceName);

            var minimumLevel = context.Configuration["Serilog:MinimumLevel:Default"];
            if (!string.IsNullOrWhiteSpace(minimumLevel)
                && Enum.TryParse(minimumLevel, true, out LogEventLevel level))
            {
                loggerConfig.MinimumLevel.Is(level);
            }
        });

        return hostBuilder;
    }

    public static IServiceCollection AddCorrelationIdConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDefaultCorrelationId();
        services.Configure<CorrelationIdOptions>(configuration.GetSection("CorrelationIdOptions"));
        return services;
    }

    public static IApplicationBuilder UseLoggingConfiguration(this IApplicationBuilder app)
    {
        app.UseCorrelationId();
        app.UseSerilogRequestLogging();
        return app;
    }

}