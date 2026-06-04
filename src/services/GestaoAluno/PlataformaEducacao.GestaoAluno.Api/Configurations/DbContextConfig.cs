using Microsoft.EntityFrameworkCore;
using PlataformaEducacao.GestaoAluno.Data;

namespace PlataformaEducacao.GestaoAluno.Api.Configurations
{
    public static class DbContextConfig
    {
        public static IServiceCollection AddDbContextConfig(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment() || environment.EnvironmentName == "Testing")
            {
                services.AddDbContext<GestaoAlunoContext>(opt =>
                {
                    opt.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
                    opt.EnableSensitiveDataLogging();
                });
            }
            else
            {
                services.AddDbContext<GestaoAlunoContext>(opt =>
                {
                    opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                    opt.EnableSensitiveDataLogging();
                });
            }
            return services;
        }
    }
}
