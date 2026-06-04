using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PlataformaEducacao.Core.Mediator;
using PlataformaEducacao.GestaoAluno.Api.Tests.Config;
using PlataformaEducacao.GestaoAluno.Data;
using PlataformaEducacao.GestaoAluno.Domain.Repositories;
using PlataformaEducacao.GestaoAluno.Domain.Services;
using PlataformaEducacao.WebApi.Core.Usuario;
using System.Net;

namespace PlataformaEducacao.GestaoAluno.Api.Tests.Configurations
{
    public class GestaoAlunoApiConfigurationsTest : IClassFixture<GestaoAlunoApiFactory>
    {
        private readonly GestaoAlunoApiFactory _factory;
        private readonly HttpClient _client;

        public GestaoAlunoApiConfigurationsTest(GestaoAlunoApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact(DisplayName = "Swagger endpoint deve retornar OK")]
        [Trait("Categoria", "GestaoAluno.Api - Configurations - SwaggerConfig")]
        public async Task SwaggerEndpoint_DeveRetornarOk()
        {
            var response = await _client.GetAsync("/swagger/v1/swagger.json");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Gestão Aluno", content);
        }

        [Fact(DisplayName = "Swagger UI deve retornar OK")]
        [Trait("Categoria", "GestaoAluno.Api - Configurations - SwaggerConfig")]
        public async Task SwaggerUI_DeveRetornarOk()
        {
            var response = await _client.GetAsync("/swagger/index.html");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "API controller sem autenticação deve retornar Unauthorized")]
        [Trait("Categoria", "GestaoAluno.Api - Configurations - ApiConfig")]
        public async Task ApiController_SemAuth_DeveRetornarUnauthorized()
        {
            var response = await _client.GetAsync("/api/alunos/matriculas-ativas");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact(DisplayName = "DependencyInjection deve registrar IAlunoRepository")]
        [Trait("Categoria", "GestaoAluno.Api - Configurations - DependencyInjectionConfig")]
        public void DI_DeveRegistrarIAlunoRepository()
        {
            using var scope = _factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IAlunoRepository>();

            Assert.NotNull(service);
        }

        [Fact(DisplayName = "DependencyInjection deve registrar IMediatorHandler")]
        [Trait("Categoria", "GestaoAluno.Api - Configurations - DependencyInjectionConfig")]
        public void DI_DeveRegistrarIMediatorHandler()
        {
            using var scope = _factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IMediatorHandler>();

            Assert.NotNull(service);
        }

        [Fact(DisplayName = "DependencyInjection deve registrar IAspNetUser")]
        [Trait("Categoria", "GestaoAluno.Api - Configurations - DependencyInjectionConfig")]
        public void DI_DeveRegistrarIAspNetUser()
        {
            using var scope = _factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IAspNetUser>();

            Assert.NotNull(service);
        }

        [Fact(DisplayName = "DependencyInjection deve registrar ICertificadoService")]
        [Trait("Categoria", "GestaoAluno.Api - Configurations - DependencyInjectionConfig")]
        public void DI_DeveRegistrarICertificadoService()
        {
            using var scope = _factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<ICertificadoService>();

            Assert.NotNull(service);
        }

        [Fact(DisplayName = "DbContext deve estar registrado e funcional")]
        [Trait("Categoria", "GestaoAluno.Api - Configurations - DbContextConfig")]
        public void DbContext_DeveEstarRegistrado()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetService<GestaoAlunoContext>();

            Assert.NotNull(context);
        }

        [Fact(DisplayName = "CORS policy deve estar configurada")]
        [Trait("Categoria", "GestaoAluno.Api - Configurations - ApiConfig")]
        public async Task CORS_DeveEstarConfigurada()
        {
            var request = new HttpRequestMessage(HttpMethod.Options, "/api/alunos/matriculas-ativas");
            request.Headers.Add("Origin", "http://example.com");
            request.Headers.Add("Access-Control-Request-Method", "GET");

            var response = await _client.SendAsync(request);

            // Should not return 405 Method Not Allowed — CORS is configured
            Assert.NotEqual(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }

        [Fact(DisplayName = "Rota inexistente deve retornar NotFound")]
        [Trait("Categoria", "GestaoAluno.Api - Configurations - ApiConfig")]
        public async Task RotaInexistente_DeveRetornarNotFound()
        {
            var response = await _client.GetAsync("/api/rota-inexistente-xyz");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
