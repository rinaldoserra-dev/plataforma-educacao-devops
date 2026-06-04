using Microsoft.EntityFrameworkCore;
using PlataformaEducacao.GestaoFinanceira.Api.Data;

namespace PlataformaEducacao.GestaoFinanceira.Api.Configuration
{
    public static class DbContextConfig
    {
        public static IServiceCollection AddDbContextConfig(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment() || environment.EnvironmentName == "Testing")
            {
                services.AddDbContext<PagamentosContext>(opt =>
                {
                    opt.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
                    opt.EnableSensitiveDataLogging();
                });
            }
            else
            {
                services.AddDbContext<PagamentosContext>(opt =>
                {
                    opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                    opt.EnableSensitiveDataLogging();
                });
            }
            return services;
        }
    }
}
