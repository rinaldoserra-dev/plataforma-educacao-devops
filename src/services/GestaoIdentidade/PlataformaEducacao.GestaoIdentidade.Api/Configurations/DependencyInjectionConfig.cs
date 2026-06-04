using PlataformaEducacao.GestaoIdentidade.Api.Data;
using PlataformaEducacao.GestaoIdentidade.Api.Data.Repository;
using PlataformaEducacao.GestaoIdentidade.Api.Services;

namespace PlataformaEducacao.GestaoIdentidade.Api.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddScoped<IAspNetUser, AspNetUser>();

            services.AddScoped<IAutenticacaoService, AutenticacaoService>();

            services.AddScoped<IAutenticacaoRepository, AutenticacaoRepository>();

            return services;
        }
    }
}
