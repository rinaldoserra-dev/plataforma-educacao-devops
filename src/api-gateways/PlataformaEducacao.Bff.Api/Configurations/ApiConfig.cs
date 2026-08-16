using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PlataformaEducacao.Bff.Api.Extensions;
using PlataformaEducacao.Bff.Api.Services;
using PlataformaEducacao.WebApi.Core.Extensions;
using PlataformaEducacao.WebApi.Core.Identidade;

namespace PlataformaEducacao.Bff.Api.Configurations
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
            services.AddControllers();

            services.Configure<AppServicesSettings>(configuration);

            services.AddCorsConfiguration(configuration);
            services.AddHealthChecks()
                .AddCheck("live", () => HealthCheckResult.Healthy(), tags: ["live"])
                .AddCheck<BffDependenciesHealthCheck>("dependencies", tags: ["ready"], timeout: TimeSpan.FromSeconds(10));
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
