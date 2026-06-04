using PlataformaEducacao.GestaoFinanceira.Api.Data;
using PlataformaEducacao.GestaoFinanceira.Api.Data.Repository;
using PlataformaEducacao.GestaoFinanceira.Api.Services;
using PlataformaEducacao.GestaoFinanceira.Business.Facade;
using PlataformaEducacao.GestaoFinanceira.Business.Models;
using PlataformaEducacao.WebApi.Core.Usuario;

namespace PlataformaEducacao.GestaoFinanceira.Api.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAspNetUser, AspNetUser>();

            services.AddScoped<IPagamentoService, PagamentoService>();
            services.AddScoped<IPagamentoCartaoCreditoFacade, PagamentoCartaoCreditoFacade>();

            services.AddScoped<IPagamentoRepository, PagamentoRepository>();
            services.AddScoped<PagamentosContext>();
        }
    }
}
