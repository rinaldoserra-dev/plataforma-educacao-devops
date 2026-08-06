using CorrelationId.HttpClient;
using PlataformaEducacao.Bff.Api.Extensions;
using PlataformaEducacao.Bff.Api.Services;
using PlataformaEducacao.WebApi.Core.Extensions;
using PlataformaEducacao.WebApi.Core.Usuario;
using Polly;

namespace PlataformaEducacao.Bff.Api.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAspNetUser, AspNetUser>();

            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

            services.AddHttpClient<IIdentidadeService, IdentidadeService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AddCorrelationIdForwarding()
                .AddPolicyHandler(PollyExtensions.EsperarTentar())
                .AddTransientHttpErrorPolicy(
                    p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            services.AddHttpClient<ICursosService, CursosService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AddCorrelationIdForwarding()
                .AddPolicyHandler(PollyExtensions.EsperarTentar())
                .AddTransientHttpErrorPolicy(
                    p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            services.AddHttpClient<IAlunosService, AlunosService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AddCorrelationIdForwarding()
                .AddPolicyHandler(PollyExtensions.EsperarTentar())
                .AddTransientHttpErrorPolicy(
                    p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            services.AddHttpClient<IPagamentoService, PagamentoService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AddCorrelationIdForwarding()
                .AddPolicyHandler(PollyExtensions.EsperarTentar())
                .AddTransientHttpErrorPolicy(
                    p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            services.AddHttpClient<IHealthCheckService, HealthCheckService>()
                .AddCorrelationIdForwarding()
                .AddPolicyHandler(PollyExtensions.EsperarTentar())
                .AddTransientHttpErrorPolicy(
                    p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            return services;
        }
    }
}
