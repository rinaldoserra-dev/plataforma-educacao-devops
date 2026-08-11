using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PlataformaEducacao.Bff.Api.Models.Request.GestaoConteudo;
using PlataformaEducacao.Bff.Api.Models.Request.Identidade;
using PlataformaEducacao.Bff.Api.Services;
using CoreResponseResult = PlataformaEducacao.Core.Communication.ResponseResult;

namespace PlataformaEducacao.Bff.Api.Tests.Config
{
    public class PlataformaEducacaoBffAppFactory<TProgram> : WebApplicationFactory<TProgram>
        where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureAppConfiguration((_, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["AppSettings:Secret"] = "c1f51f42-5727-4d15-b787-c6bbbb645024",
                    ["AppSettings:ExpiracaoHoras"] = "2",
                    ["AppSettings:ExpiracaoRefreshToken"] = "8",
                    ["AppSettings:Emissor"] = "PlataformaEducacao",
                    ["AppSettings:ValidoEm"] = "https://localhost",
                    ["IdentidadeUrl"] = "https://localhost:5431",
                    ["GestaoConteudoUrl"] = "https://localhost:5441",
                    ["GestaoAlunosUrl"] = "https://localhost:5461",
                    ["GestaoFinanceiraUrl"] = "https://localhost:5273",
                    ["MessageQueueConnection:MessageBus"] = "host=localhost:5672;publisherConfirms=true;timeout=10",
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
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

                services.RemoveAll<IIdentidadeService>();
                services.RemoveAll<ICursosService>();
                services.RemoveAll<IAlunosService>();
                services.RemoveAll<IPagamentoService>();
                services.RemoveAll<IHealthCheckService>();

                services.AddScoped<IIdentidadeService, FakeIdentidadeService>();
                services.AddScoped<ICursosService, FakeCursosService>();
                services.AddScoped<IAlunosService, FakeAlunosService>();
                services.AddScoped<IPagamentoService, FakePagamentoService>();
                services.AddScoped<IHealthCheckService, FakeHealthCheckService>();
            });
        }
    }
}
