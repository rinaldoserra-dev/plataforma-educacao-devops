using Microsoft.AspNetCore.HttpOverrides;
using PlataformaEducacao.WebApi.Core.Identidade;
using System.Text.Json.Serialization;

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
        public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
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

            services.AddCors(options =>
            {
                options.AddPolicy("Total",
                    builder =>
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
            });

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

            app.UseCors("Total");

            app.UseAuthConfiguration();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
