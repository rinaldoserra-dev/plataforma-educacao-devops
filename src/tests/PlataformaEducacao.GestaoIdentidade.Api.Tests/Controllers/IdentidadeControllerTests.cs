using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PlataformaEducacao.Core.Communication;
using PlataformaEducacao.GestaoIdentidade.Api.Controllers;
using PlataformaEducacao.GestaoIdentidade.Api.Models;
using PlataformaEducacao.GestaoIdentidade.Api.Services;
using PlataformaEducacao.WebApi.Core.Identidade;
using Xunit;

namespace PlataformaEducacao.GestaoIdentidade.Api.Tests.Controllers
{
    public class IdentidadeControllerTests
    {
        [Fact(DisplayName = "RefreshToken deve retornar BadRequest quando request for null")]
        [Trait("Categoria", "Gestão Identidade - Controllers - IdentidadeController")]
        public async Task RefreshToken_DeveRetornarBadRequest_QuandoRequestNulo()
        {
            // Arrange
            var autenticacaoService = new FakeAutenticacaoService();
            var appSettings = Options.Create(new AppSettings());
            var controller = new IdentidadeController(null!, null!, autenticacaoService, appSettings, null!);

            // Act
            var resultado = await controller.RefreshToken(null!);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
            var resposta = Assert.IsType<ResponseResult>(badRequest.Value);
            Assert.False(resposta.Sucesso);
            Assert.Contains("Refresh Token inválido", resposta.Erros.Mensagens);
        }

        [Fact(DisplayName = "RefreshToken deve retornar BadRequest quando RefreshToken string for vazia")]
        [Trait("Categoria", "Gestão Identidade - Controllers - IdentidadeController")]
        public async Task RefreshToken_DeveRetornarBadRequest_QuandoRefreshTokenVazio()
        {
            // Arrange
            var autenticacaoService = new FakeAutenticacaoService();
            var appSettings = Options.Create(new AppSettings());
            var controller = new IdentidadeController(null!, null!, autenticacaoService, appSettings, null!);

            var request = new UsuarioRefreshToken { RefreshToken = string.Empty };

            // Act
            var resultado = await controller.RefreshToken(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
            var resposta = Assert.IsType<ResponseResult>(badRequest.Value);
            Assert.False(resposta.Sucesso);
            Assert.Contains("Refresh Token inválido", resposta.Erros.Mensagens);
        }

        [Fact(DisplayName = "RefreshToken deve retornar BadRequest quando RefreshToken expirado (service retorna nulo)")]
        [Trait("Categoria", "Gestão Identidade - Controllers - IdentidadeController")]
        public async Task RefreshToken_DeveRetornarBadRequest_QuandoRefreshTokenExpirado()
        {
            // Arrange
            // Fake service configurado para retornar null em ObterRefreshToken
            var autenticacaoService = new FakeAutenticacaoService(obterRefreshTokenResult: null);
            var appSettings = Options.Create(new AppSettings());
            var controller = new IdentidadeController(null!, null!, autenticacaoService, appSettings, null!);

            var request = new UsuarioRefreshToken { RefreshToken = Guid.NewGuid().ToString() };

            // Act
            var resultado = await controller.RefreshToken(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
            var resposta = Assert.IsType<ResponseResult>(badRequest.Value);
            Assert.False(resposta.Sucesso);
            Assert.Contains("Refresh Token expirado", resposta.Erros.Mensagens);
        }

        // Fake implementation simples do IAutenticacaoService para testes unitários do controller.
        private sealed class FakeAutenticacaoService : IAutenticacaoService
        {
            private readonly RefreshToken? _obterRefreshTokenResult;

            public FakeAutenticacaoService(RefreshToken? obterRefreshTokenResult = null)
            {
                _obterRefreshTokenResult = obterRefreshTokenResult;
            }

            public Task<RefreshToken> GerarRefreshToken(string userName)
            {
                var token = new RefreshToken
                {
                    UserName = userName,
                    Token = Guid.NewGuid(),
                    ExpirationDate = DateTime.UtcNow.AddHours(1)
                };
                return Task.FromResult(token);
            }

            public Task<RefreshToken?> ObterRefreshToken(Guid refreshToken)
            {
                return Task.FromResult(_obterRefreshTokenResult);
            }
        }
    }
}
