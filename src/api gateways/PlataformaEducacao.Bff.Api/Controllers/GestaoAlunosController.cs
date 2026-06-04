using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaEducacao.Bff.Api.Models.GestaoAlunos;
using PlataformaEducacao.Bff.Api.Models.GestaoConteudo;
using PlataformaEducacao.Bff.Api.Services;
using PlataformaEducacao.WebApi.Core.Controllers;
using System.Text.Json;

namespace PlataformaEducacao.Bff.Api.Controllers
{
    [Authorize]
    [Route("alunos")]
    public class GestaoAlunosController(IAlunosService alunosService, ICursosService cursosService) : MainController
    {
        private readonly IAlunosService _alunosService = alunosService;
        private readonly ICursosService _cursoService = cursosService;

        [HttpPost("matricular")]
        public async Task<IActionResult> Matricular(MatricularDTO matricular)
        {
            if (ModelState.IsValid is false)
                return CustomResponse(ModelState);

            var curso = await _cursoService.ObterCursoComAulasPorCursoId(matricular.CursoId);

            if (!curso.Sucesso || curso.Data is null)
            {
                AdicionarErroProcessamento("Curso não encontrado");
                return CustomResponse();
            }

            var cursoDetalhes = JsonSerializer.Deserialize<CursoDetalhesDTO>(
                curso!.Data.ToString()!,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (cursoDetalhes != null)
            {
                matricular.NomeCurso = cursoDetalhes.Nome;
                matricular.Valor = cursoDetalhes.Valor;
                matricular.TotalAulasCurso = cursoDetalhes.Aulas.Count();
            }

            var resposta = await _alunosService.Matricular(matricular);

            return CustomResponse(resposta);
        }

        [HttpGet("matriculas-pendentes-pagamento")]
        public async Task<IActionResult> ObterMatriculasPendentesPagamento()
        {
            var matriculas = await _alunosService.ObterMatriculasPendentesPagamento();

            return matriculas is null ? NotFound() : CustomResponse(matriculas);
        }

        [HttpGet("matriculas-ativas")]
        public async Task<IActionResult> ObterMatriculasAtivas()
        {
            var matriculas = await _alunosService.ObterMatriculasAtivas();

            return matriculas is null ? NotFound() : CustomResponse(matriculas);
        }

        [HttpGet("validar-certificado/{codigoVerificacao}")]
        public async Task<ActionResult> ValidarCertificado(string codigoVerificacao)
        {
            var certificado = await _alunosService.ValidarCertificado(codigoVerificacao);

            return certificado is null ? NotFound() : CustomResponse(certificado);
        }

        [HttpPost("realizar-aula")]
        public async Task<IActionResult> RealizarAula(RealizarAulaDTO realizarAula)
        {
            if (ModelState.IsValid is false)
                return CustomResponse(ModelState);

            var resposta = await _alunosService.RealizarAula(realizarAula);

            return CustomResponse(resposta);
        }

        [HttpPost("finalizar-curso")]
        public async Task<IActionResult> FinalizarCurso(FinalizarCursoDTO finalizarCurso)
        {
            if (ModelState.IsValid is false)
                return CustomResponse(ModelState);

            var resposta = await _alunosService.FinalizarCurso(finalizarCurso);

            return CustomResponse(resposta);
        }

        [HttpGet("historico-aluno/{alunoId:guid}")]
        public async Task<IActionResult> ObterHistorico(Guid alunoId)
        {
            var historico = await _alunosService.ObterHistorico(alunoId);

            return historico is null ? NotFound() : CustomResponse(historico);
        }

        [HttpGet("baixar-certificado/{certificadoId:guid}")]
        public async Task<IActionResult> BaixarCertificado(Guid certificadoId)
        {
            var certificado = await _alunosService.BaixarCertificado(certificadoId);
            if (certificado.IsSuccessStatusCode is false)
                return NotFound();

            var bytes = await certificado.Content.ReadAsByteArrayAsync();
            var contentType = certificado.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            var fileName = certificado.Content.Headers.ContentDisposition?.FileNameStar
                           ?? certificado.Content.Headers.ContentDisposition?.FileName
                           ?? "certificado.pdf";

            return File(bytes, contentType, fileName.Trim('"'));
        }
    }
}
