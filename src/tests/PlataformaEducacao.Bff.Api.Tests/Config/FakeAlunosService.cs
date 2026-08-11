using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PlataformaEducacao.Bff.Api.Services;
using CoreResponseResult = PlataformaEducacao.Core.Communication.ResponseResult;
using PlataformaEducacao.Bff.Api.Models.Request.Identidade;

namespace PlataformaEducacao.Bff.Api.Tests.Config
{

    internal class FakeAlunosService : IAlunosService
    {
        public Task<HttpResponseMessage> BaixarCertificado(Guid certificadoId)
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new ByteArrayContent([1, 2, 3])
            };
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = "certificado.pdf"
            };

            return Task.FromResult(response);
        }

        public Task<CoreResponseResult> FinalizarCurso(PlataformaEducacao.Bff.Api.Models.GestaoAlunos.FinalizarCursoDTO finalizarCurso)
            => Ok(new { finalizarCurso.MatriculaId });

        public Task<CoreResponseResult> Matricular(PlataformaEducacao.Bff.Api.Models.GestaoAlunos.MatricularDTO solicitarMatricula)
        {
            if (string.IsNullOrWhiteSpace(solicitarMatricula.NomeCurso))
            {
                solicitarMatricula.NomeCurso = "Curso de Microsservicos";
                solicitarMatricula.TotalAulasCurso = 2;
                solicitarMatricula.Valor = 199.90m;
            }

            return Ok(new
            {
                solicitarMatricula.CursoId,
                solicitarMatricula.NomeCurso,
                solicitarMatricula.TotalAulasCurso,
                solicitarMatricula.Valor
            });
        }

        public Task<CoreResponseResult> ObterHistorico(Guid alunoId)
            => Ok(new { AlunoId = alunoId, CursosConcluidos = 1 });

        public Task<CoreResponseResult> ObterMatriculasAtivas()
            => Ok(new[] { new { MatriculaId = Guid.NewGuid(), Status = "Ativa" } });

        public Task<CoreResponseResult> ObterMatriculasPendentesPagamento()
            => Ok(new[] { new { MatriculaId = Guid.NewGuid(), Status = "PendentePagamento" } });

        public Task<CoreResponseResult> RealizarAula(PlataformaEducacao.Bff.Api.Models.GestaoAlunos.RealizarAulaDTO realizarAula)
            => Ok(new { realizarAula.MatriculaId, realizarAula.AulaId });

        public Task<CoreResponseResult> ValidarCertificado(string codigoVerificacao)
            => Ok(new { Codigo = codigoVerificacao, Valido = true });

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
