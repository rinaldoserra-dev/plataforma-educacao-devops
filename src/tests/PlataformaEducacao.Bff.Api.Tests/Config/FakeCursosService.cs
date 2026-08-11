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
    internal class FakeCursosService : ICursosService
    {
        public Task<CoreResponseResult> AdicionarAula(AdicionarAulaRequest aulaRequest)
            => Ok(new { aulaRequest.CursoId, aulaRequest.Titulo });

        public Task<CoreResponseResult> AdicionarCurso(AdicionarCursoRequest cursoRequest)
            => Ok(new { cursoRequest.Nome });

        public Task<CoreResponseResult> AtualizarCurso(Guid cursoId, AtualizarCursoRequest cursoRequest)
            => Ok(new { cursoId, cursoRequest.Nome });

        public Task<CoreResponseResult> ObterCursoComAulasPorCursoId(Guid cursoId)
            => Ok(new
            {
                Id = cursoId,
                Nome = "Curso de Microsservicos",
                Valor = 199.90m,
                Disponivel = true,
                Aulas = new[]
                {
                    new { Id = Guid.NewGuid() },
                    new { Id = Guid.NewGuid() }
                }
            });

        public Task<CoreResponseResult> ObterCursosDisponiveisComAula()
            => Ok(new[]
            {
                new
                {
                    Id = Guid.NewGuid(),
                    Nome = "Curso de Microsservicos",
                    Valor = 199.90m
                }
            });

        public Task<CoreResponseResult> ObterTodos()
            => ObterCursosDisponiveisComAula();

        private static Task<CoreResponseResult> Ok(object data)
        {
            return Task.FromResult(new CoreResponseResult
            {
                Sucesso = true,
                Status = StatusCodes.Status200OK,
                Data = data
            });
        }
    }
}
