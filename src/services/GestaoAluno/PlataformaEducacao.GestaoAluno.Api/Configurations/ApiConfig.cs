using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;
using PlataformaEducacao.WebApi.Core.Extensions;
using PlataformaEducacao.WebApi.Core.Identidade;

namespace PlataformaEducacao.GestaoAluno.Api.Configurations
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
            services.AddHealthChecks();

            return services;
        }

        public static void UseApiConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            if (env.IsEnvironment("Docker") is false)
                app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("Frontend");

            app.UseAuthConfiguration();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
                endpoints.MapHealthChecks("/health/ready");
            });
        }
    }
}
