using Microsoft.EntityFrameworkCore;
using PlataformaEducacao.GestaoIdentidade.Api.Data;

namespace PlataformaEducacao.GestaoIdentidade.Api.Configurations
{
    public static class DbContextConfig
    {
        public static IServiceCollection AddDbContextConfig(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment() || environment.EnvironmentName == "Testing")
            {
                services.AddDbContext<GestaoIdentidadeContext>(opt =>
                {
                    opt.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
                    opt.EnableSensitiveDataLogging();
                });
            }
            else
            {
                services.AddDbContext<GestaoIdentidadeContext>(opt =>
                {
                    opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                    opt.EnableSensitiveDataLogging();
                });
            }
            return services;
        }
    }
}
