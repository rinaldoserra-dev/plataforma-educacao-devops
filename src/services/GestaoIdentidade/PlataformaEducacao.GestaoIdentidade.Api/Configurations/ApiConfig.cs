using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PlataformaEducacao.GestaoIdentidade.Api.Data;
using PlataformaEducacao.MessageBus.HealthChecks;
using PlataformaEducacao.WebApi.Core.Extensions;
using PlataformaEducacao.WebApi.Core.HealthChecks;
using PlataformaEducacao.WebApi.Core.Identidade;

namespace PlataformaEducacao.GestaoIdentidade.Api.Configurations
{
    public static class ApiConfig
    {
        public static IHostBuilder ConfigureAppSettings(this IHostBuilder host)
        {
            host.ConfigureAppConfiguration((ctx, builder) =>
            {
                var enviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                builder.SetBasePath(Directory.GetCurrentDirectory());
                builder.AddJsonFile("appsettings.json", true, true);
                builder.AddJsonFile($"appsettings.{enviroment}.json", true, true);

                builder.AddEnvironmentVariables();
            });

            return host;
        }

        public static IServiceCollection AddApiConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers()
                .ConfigureApiBehaviorOptions(opt => opt.SuppressModelStateInvalidFilter = true)
                .AddJsonOptions(option =>
                {
                    option.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    option.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
            services.AddCorsConfiguration(configuration);
            services.AddHealthChecks()
                .AddCheck("live", () => HealthCheckResult.Healthy(), tags: ["live"])
                .AddCheck<DatabaseHealthCheck<GestaoIdentidadeContext>>("sqlserver", tags: ["ready"], timeout: TimeSpan.FromSeconds(5))
                .AddCheck<MessageBusHealthCheck>("rabbitmq", tags: ["ready"], timeout: TimeSpan.FromSeconds(5));

            return services;
        }

        public static IApplicationBuilder UseApiConfiguration(this IApplicationBuilder app, IWebHostEnvironment environment)
        {
            app.UseForwardedHeaders();
            if (environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (!environment.IsEnvironment("Docker"))
                app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("Frontend");

            app.UseAuthConfiguration();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains("live")
                });
                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains("ready")
                });
            });

            return app;
        }
    }
}
