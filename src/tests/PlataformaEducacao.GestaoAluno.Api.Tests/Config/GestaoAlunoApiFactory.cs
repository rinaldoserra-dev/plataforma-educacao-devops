using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlataformaEducacao.GestaoAluno.Data;
using static PlataformaEducacao.GestaoAluno.Api.Configurations.DbMigrationHelperExtension;

namespace PlataformaEducacao.GestaoAluno.Api.Tests.Config
{
    public class GestaoAlunoApiFactory : WebApplicationFactory<Program>, IDisposable
    {
        private SqliteConnection _connection = null!;

        public GestaoAlunoApiFactory()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContextTypes = new[]
                {
                    typeof(GestaoAlunoContext)
                };

                foreach (var dbContextType in dbContextTypes)
                {
                    var descriptorsToRemove = services
                        .Where(d =>
                            d.ServiceType == dbContextType ||
                            (d.ServiceType.IsGenericType &&
                             d.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>) &&
                             d.ServiceType.GenericTypeArguments[0] == dbContextType) ||
                            d.ServiceType == typeof(DbContextOptions))
                        .ToList();

                    foreach (var descriptor in descriptorsToRemove)
                    {
                        services.Remove(descriptor);
                    }
                }

                services.AddDbContext<GestaoAlunoContext>(options => options.UseSqlite(_connection));

                var hostedServices = services.Where(
                    d => d.ServiceType == typeof(IHostedService)).ToList();
                foreach (var hs in hostedServices)
                    services.Remove(hs);

                using (var scope = services.BuildServiceProvider().CreateScope())
                {
                    var serviceProvider = scope.ServiceProvider;
                    DbMigrationHelper.EnsureSeedData(serviceProvider).GetAwaiter().GetResult();
                }
            });
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            return base.CreateHost(builder);
        }

        public new void Dispose()
        {
            base.Dispose();

            if (_connection != null)
            {
                _connection.Close();
            }
        }
    }
}
