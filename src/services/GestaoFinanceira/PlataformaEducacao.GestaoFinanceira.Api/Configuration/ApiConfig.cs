using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PlataformaEducacao.GestaoFinanceira.Api.Data;
using PlataformaEducacao.MessageBus.HealthChecks;
using PlataformaEducacao.WebApi.Core.Extensions;
using PlataformaEducacao.WebApi.Core.HealthChecks;
using PlataformaEducacao.WebApi.Core.Identidade;

namespace PlataformaEducacao.GestaoFinanceira.Api.Configuration
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

        public static IServiceCollection AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
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

            services.AddControllers();

            services.AddCorsConfiguration(configuration);
            services.AddHealthChecks()
                .AddCheck("live", () => HealthCheckResult.Healthy(), tags: ["live"])
                .AddCheck<DatabaseHealthCheck<PagamentosContext>>("sqlserver", tags: ["ready"])
                .AddCheck<MessageBusHealthCheck>("rabbitmq", tags: ["ready"]);

            return services;
        }

        public static void UseApiConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
        }
    }
}
