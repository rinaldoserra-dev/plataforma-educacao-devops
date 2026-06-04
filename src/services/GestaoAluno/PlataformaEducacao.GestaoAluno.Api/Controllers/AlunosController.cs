using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaEducacao.Core.Mediator;
using PlataformaEducacao.GestaoAluno.Application.Commands.FinalizarCurso;
using PlataformaEducacao.GestaoAluno.Application.Commands.MatricularAlunoCurso;
using PlataformaEducacao.GestaoAluno.Application.Commands.RealizarAula;
using PlataformaEducacao.GestaoAluno.Application.DTO;
using PlataformaEducacao.GestaoAluno.Application.Queries;
using PlataformaEducacao.GestaoAluno.Application.Queries.ViewModels;
using PlataformaEducacao.WebApi.Core.Controllers;
using PlataformaEducacao.WebApi.Core.Usuario;
using System.Net;

namespace PlataformaEducacao.GestaoAluno.Api.Controllers
{
    [Route("api/alunos")]
    public class AlunosController : MainController
    {
        private readonly IAlunoQueries _alunoQueries;
        private readonly IAspNetUser _user;
        private readonly IMediatorHandler _mediatorHandler;

        public AlunosController(IAlunoQueries alunoQueries,
                                IAspNetUser user,
                                IMediatorHandler mediatorHandler)
        {
            _alunoQueries = alunoQueries;
            _user = user;
            _mediatorHandler = mediatorHandler;
        }

        [HttpGet("matriculas-ativas")]
        [Authorize(Roles = "ALUNO")]
        public async Task<ActionResult<IEnumerable<MatriculaViewModel>>> ObterMatriculasAtivas(CancellationToken cancellationToken)
        {
            var matriculas = await _alunoQueries.ObterMatriculasAtivasPorAlunoId(_user.ObterUserId(), cancellationToken);
            return CustomResponse(HttpStatusCode.OK, matriculas);
        }

        [HttpGet("matriculas-pendentes-pagamento")]
        [Authorize(Roles = "ALUNO")]
        public async Task<ActionResult<IEnumerable<MatriculaViewModel>>> ObterMatriculasPendentesPagamento(CancellationToken cancellationToken)
        {
            var matriculas = await _alunoQueries.ListarMatriculasPendentesPagamentoPorAlunoId(_user.ObterUserId(), cancellationToken);
            return CustomResponse(HttpStatusCode.OK, matriculas);
        }

        [HttpPost("matricular")]
        [Authorize(Roles = "ALUNO")]
        public async Task<IActionResult> Matricular([FromBody] MatricularAlunoCursoCommand matricularAlunoCurso, CancellationToken cancellationToken)
        {
            matricularAlunoCurso.VincularAluno(_user.ObterUserId());

            if (matricularAlunoCurso.EhValido() is false)
                return CustomResponse(matricularAlunoCurso.ValidationResult);

            return CustomResponse(await _mediatorHandler.SendCommand(matricularAlunoCurso));
        }

        [HttpPost("realizar-aula")]
        [Authorize(Roles = "ALUNO")]
        public async Task<IActionResult> RealizarAula([FromBody] RealizarAulaCommand realizarAulaCommand, CancellationToken cancellationToken)
        {
            if (realizarAulaCommand.EhValido() is false)
                return CustomResponse(realizarAulaCommand.ValidationResult);

            return CustomResponse(await _mediatorHandler.SendCommand(realizarAulaCommand));
        }

        [HttpPost("finalizar-curso")]
        [Authorize(Roles = "ALUNO")]
        public async Task<IActionResult> FinalizarCurso([FromBody] FinalizarCursoCommand finalizarCursoCommand, CancellationToken cancellationToken)
        {
            if (finalizarCursoCommand.EhValido() is false)
                return CustomResponse(finalizarCursoCommand.ValidationResult);

            return CustomResponse(await _mediatorHandler.SendCommand(finalizarCursoCommand));
        }

        [HttpGet("validar-certificado/{codigoVerificacao}")]
        [AllowAnonymous]
        public async Task<ActionResult<CertificadoDTO>> ValidarCertificado(string codigoVerificacao, CancellationToken cancellationToken)
        {
            var certificado = await _alunoQueries.ValidarCertificado(codigoVerificacao, cancellationToken);

            return CustomResponse(HttpStatusCode.OK, certificado);
        }

        [HttpGet("baixar-certificado/{certificadoId:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult> BaixarCertificado(Guid certificadoId, CancellationToken cancellationToken)
        {
            var certificado = await _alunoQueries.BaixarCertificado(certificadoId, cancellationToken);

            if (certificado is null)
            {
                AdicionarErroProcessamento("Certificado não encontrado.");
                return CustomResponse();
            }
            return File(certificado.PdfBytes, certificado.ContentType, certificado.NomeArquivo);
        }

        [HttpGet("historico-aluno/{alunoId:guid}")]
        [Authorize(Roles = "ADMIN,ALUNO")]
        public async Task<ActionResult<HistoricoAlunoViewModel>> ObterHistoricoAluno(Guid alunoId, CancellationToken cancellationToken)
        {
            var isAdm = _user.PossuiRole("ADMIN");

            var alunoAutenticado = _user.ObterUserId();

            if (isAdm is false && alunoId != alunoAutenticado)
            {
                AdicionarErroProcessamento("Um aluno não pode obter histórico de outro aluno.");
                return CustomResponse();
            }

            var historicoAluno = await _alunoQueries.ObterHistoricoAluno(alunoId, cancellationToken);

            if (historicoAluno is null)
            {
                AdicionarErroProcessamento("Histórico de aluno não encontrado.");
                return CustomResponse();
            }
            return CustomResponse(HttpStatusCode.OK, historicoAluno);
        }
    }
}