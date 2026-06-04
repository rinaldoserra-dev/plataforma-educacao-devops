using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlataformaEducacao.GestaoFinanceira.Api.Data;
using static PlataformaEducacao.GestaoFinanceira.Api.Configuration.DbMigrationHelperExtension;

namespace PlataformaEducacao.GestaoFinanceira.Api.Tests.Config
{
    public class GestaoFinanceiraApiFactory : WebApplicationFactory<Program>, IDisposable
    {
        private SqliteConnection _connection = null!;

        public GestaoFinanceiraApiFactory()
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
                    typeof(PagamentosContext)
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

                services.AddDbContext<PagamentosContext>(options => options.UseSqlite(_connection));

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
