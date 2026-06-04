using Microsoft.AspNetCore.Identity;
using PlataformaEducacao.GestaoIdentidade.Api.Data;
using PlataformaEducacao.GestaoIdentidade.Api.Extensions;
using PlataformaEducacao.WebApi.Core.Identidade;

namespace PlataformaEducacao.GestaoIdentidade.Api.Configurations
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddIdentityConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddRoles<IdentityRole>()
                .AddErrorDescriber<IdentityPortuguesMsgError>()
                .AddEntityFrameworkStores<GestaoIdentidadeContext>()
                .AddDefaultTokenProviders();


            services.AddJwtConfiguration(configuration);

            return services;
        }
    }
}
