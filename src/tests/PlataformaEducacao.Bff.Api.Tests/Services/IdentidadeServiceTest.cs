using Microsoft.Extensions.Options;
using PlataformaEducacao.Bff.Api.Extensions;
using PlataformaEducacao.Bff.Api.Models.Request.Identidade;
using PlataformaEducacao.Bff.Api.Services;
using PlataformaEducacao.Core.Communication;
using System.Net;

namespace PlataformaEducacao.Bff.Api.Tests.Services
{
    public class IdentidadeServiceTest
    {
        private readonly MockHttpMessageHandler _handler;
        private readonly IdentidadeService _service;

        public IdentidadeServiceTest()
        {
            _handler = new MockHttpMessageHandler();
            var httpClient = new HttpClient(_handler) { BaseAddress = new Uri("http://localhost") };
            var settings = Options.Create(new AppServicesSettings { IdentidadeUrl = "http://localhost" });
            _service = new IdentidadeService(httpClient, settings);
        }

        [Fact(DisplayName = "Login deve retornar ResponseResult")]
        [Trait("Categoria", "Bff.Api - Services - IdentidadeService")]
        public async Task Login_DeveRetornarResponseResult()
        {
            _handler.SetupResponse(HttpStatusCode.OK, new ResponseResult { Sucesso = true, Status = 200, Erros = new ResponseErrorMessages() });
            var login = new LoginRequest { Email = "user@test.com", Senha = "Senha@123" };

            var result = await _service.Login(login);

            Assert.True(result.Sucesso);
        }

        [Fact(DisplayName = "RegistrarAluno deve retornar ResponseResult")]
        [Trait("Categoria", "Bff.Api - Services - IdentidadeService")]
        public async Task RegistrarAluno_DeveRetornarResponseResult()
        {
            _handler.SetupResponse(HttpStatusCode.OK, new ResponseResult { Sucesso = true, Status = 200, Erros = new ResponseErrorMessages() });
            var registro = new RegistroAlunoRequest { Nome = "Aluno", Email = "aluno@test.com", Senha = "Senha@123", SenhaConfirmacao = "Senha@123" };

            var result = await _service.RegistrarAluno(registro);

            Assert.True(result.Sucesso);
        }

        [Fact(DisplayName = "Login com falha deve retornar erros")]
        [Trait("Categoria", "Bff.Api - Services - IdentidadeService")]
        public async Task Login_ComFalha_DeveRetornarErros()
        {
            _handler.SetupResponse(HttpStatusCode.BadRequest, new ResponseResult
            {
                Sucesso = false,
                Status = 400,
                Erros = new ResponseErrorMessages { Mensagens = ["Credenciais inválidas."] }
            });
            var login = new LoginRequest { Email = "user@test.com", Senha = "errada" };

            var result = await _service.Login(login);

            Assert.False(result.Sucesso);
        }
    }
}
