using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PlataformaEducacao.Bff.Api.Extensions;
using PlataformaEducacao.Bff.Api.Services;
using System.Net;

namespace PlataformaEducacao.Bff.Api.Tests.Services
{
    public class HealthCheckServiceTest
    {
        private readonly AppServicesSettings _settings;

        public HealthCheckServiceTest()
        {
            _settings = new AppServicesSettings
            {
                IdentidadeUrl = "http://localhost:5001",
                GestaoConteudoUrl = "http://localhost:5002",
                GestaoAlunosUrl = "http://localhost:5003",
                GestaoFinanceiraUrl = "http://localhost:5004"
            };
        }

        [Fact(DisplayName = "VerificarSaude quando todos serviços saudáveis deve retornar sucesso")]
        [Trait("Categoria", "Bff.Api - Services - HealthCheckService")]
        public async Task VerificarSaude_TodosSaudaveis_DeveRetornarSucesso()
        {
            // Arrange
            var handler = new MockHttpMessageHandler();
            handler.SetupResponse(HttpStatusCode.OK);

            var httpClient = new HttpClient(handler);
            var options = Options.Create(_settings);
            var service = new HealthCheckService(httpClient, options);

            // Act
            var resultado = await service.VerificarSaude();

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Status.Should().Be(StatusCodes.Status200OK);
        }

        [Fact(DisplayName = "VerificarSaude quando um serviço falha deve retornar 503")]
        [Trait("Categoria", "Bff.Api - Services - HealthCheckService")]
        public async Task VerificarSaude_UmServicoFalha_DeveRetornar503()
        {
            // Arrange
            var handler = new MockHttpMessageHandler();
            handler.SetupResponse("localhost:5001", HttpStatusCode.OK);
            handler.SetupResponse("localhost:5002", HttpStatusCode.OK);
            handler.SetupResponse("localhost:5003", HttpStatusCode.OK);
            handler.SetupResponse("localhost:5004", HttpStatusCode.ServiceUnavailable);

            var httpClient = new HttpClient(handler);
            var options = Options.Create(_settings);
            var service = new HealthCheckService(httpClient, options);

            // Act
            var resultado = await service.VerificarSaude();

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Status.Should().Be(StatusCodes.Status503ServiceUnavailable);
            resultado.Erros.Mensagens.Should().Contain("Uma ou mais dependencias estao indisponiveis.");
        }

        [Fact(DisplayName = "VerificarSaude quando serviço lança exceção deve retornar indisponível")]
        [Trait("Categoria", "Bff.Api - Services - HealthCheckService")]
        public async Task VerificarSaude_ServicoLancaExcecao_DeveRetornarIndisponivel()
        {
            // Arrange
            var handler = new ExceptionThrowingHandler();
            var httpClient = new HttpClient(handler);
            var options = Options.Create(_settings);
            var service = new HealthCheckService(httpClient, options);

            // Act
            var resultado = await service.VerificarSaude();

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Status.Should().Be(StatusCodes.Status503ServiceUnavailable);
        }

        [Fact(DisplayName = "VerificarSaude deve retornar dados de todas as dependências")]
        [Trait("Categoria", "Bff.Api - Services - HealthCheckService")]
        public async Task VerificarSaude_DeveRetornarDadosDeTodasDependencias()
        {
            // Arrange
            var handler = new MockHttpMessageHandler();
            handler.SetupResponse(HttpStatusCode.OK);

            var httpClient = new HttpClient(handler);
            var options = Options.Create(_settings);
            var service = new HealthCheckService(httpClient, options);

            // Act
            var resultado = await service.VerificarSaude();

            // Assert
            resultado.Data.Should().NotBeNull();
        }

        private class ExceptionThrowingHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                throw new HttpRequestException("Serviço indisponível");
            }
        }
    }
}
