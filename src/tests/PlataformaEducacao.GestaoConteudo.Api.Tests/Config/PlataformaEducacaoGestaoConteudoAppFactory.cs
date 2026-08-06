using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlataformaEducacao.GestaoConteudo.Data;
using static PlataformaEducacao.GestaoConteudo.Api.Configurations.DbMigrationHelperExtension;

namespace PlataformaEducacao.GestaoConteudo.Api.Tests.Config
{
    public class PlataformaEducacaoGestaoConteudoAppFactory<TProgram> : WebApplicationFactory<TProgram>, IDisposable where TProgram : class
    {
        private SqliteConnection _connection = null!;
        public PlataformaEducacaoGestaoConteudoAppFactory()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((_, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["CorrelationIdOptions:RequestHeader"] = "X-Correlation-ID",
                    ["CorrelationIdOptions:IncludeInResponse"] = "true",
                    ["CorrelationIdOptions:AddToLoggingScope"] = "false",
                    ["Serilog:MinimumLevel:Default"] = "Error",
                    ["Serilog:MinimumLevel:Override:Microsoft"] = "Error",
                    ["Serilog:MinimumLevel:Override:Microsoft.Hosting.Lifetime"] = "Error",
                    ["Serilog:MinimumLevel:Override:System"] = "Error",
                    ["Serilog:WriteTo:0:Name"] = "Debug",
                    ["Serilog:WriteTo:1:Name"] = "Debug"
                });
            });
            builder.ConfigureServices(services =>
            {
                var dbContextTypes = new[]
                {
                    typeof(GestaoConteudoContext)
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

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

                services.AddDbContext<GestaoConteudoContext>(options => options.UseSqlite(_connection));


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
