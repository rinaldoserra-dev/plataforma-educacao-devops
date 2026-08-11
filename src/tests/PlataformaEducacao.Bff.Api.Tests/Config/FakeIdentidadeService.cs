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
    internal sealed class FakeIdentidadeService : IIdentidadeService
    {
        public Task<CoreResponseResult> Login(LoginRequest login)
        {
            return Task.FromResult(new CoreResponseResult
            {
                Sucesso = true,
                Status = StatusCodes.Status200OK,
                Data = new
                {
                    accessToken = "fake-token",
                    expiresIn = 7200
                }
            });
        }

        public Task<CoreResponseResult> RegistrarAluno(RegistroAlunoRequest aluno)
        {
            return Task.FromResult(new CoreResponseResult
            {
                Sucesso = true,
                Status = StatusCodes.Status201Created,
                Data = new { aluno.Email }
            });
        }

        public Task<CoreResponseResult> RefreshToken(RefreshTokenRequest refreshToken)
        {
            return Task.FromResult(new CoreResponseResult
            {
                Sucesso = true,
                Status = StatusCodes.Status200OK,
                Data = new
                {
                    RefreshToken = "fake-refresh-token"
                }
            });
        }
    }
}
