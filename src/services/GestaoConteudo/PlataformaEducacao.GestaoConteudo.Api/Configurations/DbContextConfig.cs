using Microsoft.EntityFrameworkCore;
using PlataformaEducacao.GestaoConteudo.Data;

namespace PlataformaEducacao.GestaoConteudo.Api.Configurations
{
    public static class DbContextConfig
    {
        public static IServiceCollection AddDbContextConfig(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment() || environment.EnvironmentName == "Testing")
            {
                services.AddDbContext<GestaoConteudoContext>(opt =>
                {
                    opt.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
                    opt.EnableSensitiveDataLogging();
                });
            }
            else
            {
                services.AddDbContext<GestaoConteudoContext>(opt =>
                {
                    opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                    opt.EnableSensitiveDataLogging();
                });
            }
            return services;
        }
    }
}
