using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PlataformaEducacao.GestaoFinanceira.Api.Data;
using PlataformaEducacao.GestaoFinanceira.Api.Services;
using PlataformaEducacao.GestaoFinanceira.Api.Tests.Config;
using PlataformaEducacao.GestaoFinanceira.Business.Facade;
using PlataformaEducacao.GestaoFinanceira.Business.Models;
using PlataformaEducacao.WebApi.Core.Usuario;
using System.Net;

namespace PlataformaEducacao.GestaoFinanceira.Api.Tests.Configurations
{
    public class GestaoFinanceiraApiConfigurationsTest : IClassFixture<GestaoFinanceiraApiFactory>
    {
        private readonly GestaoFinanceiraApiFactory _factory;
        private readonly HttpClient _client;

        public GestaoFinanceiraApiConfigurationsTest(GestaoFinanceiraApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact(DisplayName = "Swagger endpoint deve retornar OK")]
        [Trait("Categoria", "GestaoFinanceira.Api - Configurations - SwaggerConfig")]
        public async Task SwaggerEndpoint_DeveRetornarOk()
        {
            var response = await _client.GetAsync("/swagger/v1/swagger.json");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Pagamento", content);
        }

        [Fact(DisplayName = "Swagger UI deve retornar OK")]
        [Trait("Categoria", "GestaoFinanceira.Api - Configurations - SwaggerConfig")]
        public async Task SwaggerUI_DeveRetornarOk()
        {
            var response = await _client.GetAsync("/swagger/index.html");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "API controller sem autenticação deve retornar Unauthorized")]
        [Trait("Categoria", "GestaoFinanceira.Api - Configurations - ApiConfig")]
        public async Task ApiController_SemAuth_DeveRetornarUnauthorized()
        {
            var response = await _client.GetAsync($"/api/Pagamento/{Guid.NewGuid()}/status");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact(DisplayName = "DependencyInjection deve registrar IPagamentoService")]
        [Trait("Categoria", "GestaoFinanceira.Api - Configurations - DependencyInjectionConfig")]
        public void DI_DeveRegistrarIPagamentoService()
        {
            using var scope = _factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IPagamentoService>();

            Assert.NotNull(service);
        }

        [Fact(DisplayName = "DependencyInjection deve registrar IPagamentoRepository")]
        [Trait("Categoria", "GestaoFinanceira.Api - Configurations - DependencyInjectionConfig")]
        public void DI_DeveRegistrarIPagamentoRepository()
        {
            using var scope = _factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IPagamentoRepository>();

            Assert.NotNull(service);
        }

        [Fact(DisplayName = "DependencyInjection deve registrar IPagamentoCartaoCreditoFacade")]
        [Trait("Categoria", "GestaoFinanceira.Api - Configurations - DependencyInjectionConfig")]
        public void DI_DeveRegistrarIPagamentoCartaoCreditoFacade()
        {
            using var scope = _factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IPagamentoCartaoCreditoFacade>();

            Assert.NotNull(service);
        }

        [Fact(DisplayName = "DependencyInjection deve registrar IAspNetUser")]
        [Trait("Categoria", "GestaoFinanceira.Api - Configurations - DependencyInjectionConfig")]
        public void DI_DeveRegistrarIAspNetUser()
        {
            using var scope = _factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IAspNetUser>();

            Assert.NotNull(service);
        }

        [Fact(DisplayName = "DbContext deve estar registrado e funcional")]
        [Trait("Categoria", "GestaoFinanceira.Api - Configurations - DbContextConfig")]
        public void DbContext_DeveEstarRegistrado()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetService<PagamentosContext>();

            Assert.NotNull(context);
        }

        [Fact(DisplayName = "Rota inexistente deve retornar NotFound")]
        [Trait("Categoria", "GestaoFinanceira.Api - Configurations - ApiConfig")]
        public async Task RotaInexistente_DeveRetornarNotFound()
        {
            var response = await _client.GetAsync("/api/rota-inexistente-xyz");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
