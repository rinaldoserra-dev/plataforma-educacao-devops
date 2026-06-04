using Microsoft.Extensions.Options;
using PlataformaEducacao.Bff.Api.Extensions;
using PlataformaEducacao.Bff.Api.Models.GestaoFinanceira;
using PlataformaEducacao.Bff.Api.Services;
using PlataformaEducacao.Core.Communication;
using System.Net;

namespace PlataformaEducacao.Bff.Api.Tests.Services
{
    public class BffPagamentoServiceTest
    {
        private readonly MockHttpMessageHandler _handler;
        private readonly PagamentoService _service;

        public BffPagamentoServiceTest()
        {
            _handler = new MockHttpMessageHandler();
            var httpClient = new HttpClient(_handler) { BaseAddress = new Uri("http://localhost") };
            var settings = Options.Create(new AppServicesSettings { GestaoFinanceiraUrl = "http://localhost" });
            _service = new PagamentoService(httpClient, settings);
        }

        [Fact(DisplayName = "PagarMatricula deve retornar ResponseResult")]
        [Trait("Categoria", "Bff.Api - Services - PagamentoService")]
        public async Task PagarMatricula_DeveRetornarResponseResult()
        {
            _handler.SetupResponse(HttpStatusCode.OK, new ResponseResult { Sucesso = true, Status = 200, Erros = new ResponseErrorMessages() });
            var dto = new PagarMatriculaDTO
            {
                MatriculaId = Guid.NewGuid(),
                AlunoId = Guid.NewGuid(),
                Valor = 100m,
                NomeCartao = "Fulano",
                NumeroCartao = "4111111111111111",
                ExpiracaoCartao = "12/2030",
                CvvCartao = "123"
            };

            var result = await _service.PagarMatricula(dto);

            Assert.True(result.Sucesso);
        }

        [Fact(DisplayName = "ObterStatus deve retornar ResponseResult")]
        [Trait("Categoria", "Bff.Api - Services - PagamentoService")]
        public async Task ObterStatus_DeveRetornarResponseResult()
        {
            _handler.SetupResponse(HttpStatusCode.OK, new ResponseResult { Sucesso = true, Status = 200, Erros = new ResponseErrorMessages() });

            var result = await _service.ObterStatus(Guid.NewGuid());

            Assert.True(result.Sucesso);
        }

        [Fact(DisplayName = "HealthCheck deve retornar ResponseResult")]
        [Trait("Categoria", "Bff.Api - Services - PagamentoService")]
        public async Task HealthCheck_DeveRetornarResponseResult()
        {
            _handler.SetupResponse(HttpStatusCode.OK, new ResponseResult { Sucesso = true, Status = 200, Erros = new ResponseErrorMessages() });

            var result = await _service.HealthCheck();

            Assert.True(result.Sucesso);
        }

        [Fact(DisplayName = "PagarMatricula com erro deve retornar falha")]
        [Trait("Categoria", "Bff.Api - Services - PagamentoService")]
        public async Task PagarMatricula_ComErro_DeveRetornarFalha()
        {
            _handler.SetupResponse(HttpStatusCode.BadRequest, new ResponseResult
            {
                Sucesso = false,
                Status = 400,
                Erros = new ResponseErrorMessages { Mensagens = ["Pagamento recusado."] }
            });
            var dto = new PagarMatriculaDTO
            {
                MatriculaId = Guid.NewGuid(),
                AlunoId = Guid.NewGuid(),
                Valor = 100m,
                NomeCartao = "Fulano",
                NumeroCartao = "4111111111111111",
                ExpiracaoCartao = "12/2030",
                CvvCartao = "123"
            };

            var result = await _service.PagarMatricula(dto);

            Assert.False(result.Sucesso);
        }
    }
}
