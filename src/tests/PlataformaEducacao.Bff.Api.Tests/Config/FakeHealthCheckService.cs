using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PlataformaEducacao.Bff.Api.Models.Request.Identidade;
using PlataformaEducacao.Bff.Api.Services;
using CoreResponseResult = PlataformaEducacao.Core.Communication.ResponseResult;

namespace PlataformaEducacao.Bff.Api.Tests.Config
{
    internal class FakeHealthCheckService : IHealthCheckService
    {
        public Task<CoreResponseResult> VerificarSaude()
        {
            return Task.FromResult(new CoreResponseResult
            {
                Sucesso = true,
                Status = StatusCodes.Status200OK,
                Data = new
                {
                    Gateway = "PlataformaEducacao.Bff.Api",
                    Dependencias = new[]
                    {
                        new { Servico = "Identidade", Saudavel = true },
                        new { Servico = "Gestao de Conteudo", Saudavel = true },
                        new { Servico = "Gestao de Alunos", Saudavel = true },
                        new { Servico = "Gestao Financeira", Saudavel = true }
                    }
                }
            });
        }
    }
}
