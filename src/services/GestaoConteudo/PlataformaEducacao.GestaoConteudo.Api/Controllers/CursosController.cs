using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaEducacao.Core.Mediator;
using PlataformaEducacao.GestaoConteudo.Api.Requests;
using PlataformaEducacao.GestaoConteudo.Application.Commands;
using PlataformaEducacao.GestaoConteudo.Application.Queries;
using PlataformaEducacao.GestaoConteudo.Application.Queries.ViewModels;
using PlataformaEducacao.WebApi.Core.Controllers;
using System.Net;

namespace PlataformaEducacao.GestaoConteudo.Api.Controllers
{
    [Route("api/cursos")]
    public class CursosController : MainController
    {
        private readonly ICursoQueries _cursoQueries;
        private readonly IMediatorHandler _mediatorHandler;
        public CursosController(ICursoQueries cursoQueries,
                                IMediatorHandler mediatorHandler)
        {
            _cursoQueries = cursoQueries;
            _mediatorHandler = mediatorHandler;
        }
        [AllowAnonymous]
        [HttpGet("listar-cursos-disponiveis")]
        public async Task<ActionResult<IEnumerable<CursoViewModel>>> ListarCursosDisponiveisParaMatricula(CancellationToken cancellationToken)
        {
            var cursos = await _cursoQueries.ObterDisponiveisComAula(cancellationToken);
            return CustomResponse(HttpStatusCode.OK, cursos);
        }

        [HttpGet("{cursoId:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<CursoViewModel>> ObterDetalhesCurso(Guid cursoId, CancellationToken cancellationToken)
        {
            var curso = await _cursoQueries.ObterCursoComAulasPorCursoId(cursoId, cancellationToken);
            return CustomResponse(HttpStatusCode.OK, curso);
        }

        [HttpGet("listar-todos-cursos")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<IEnumerable<CursoViewModel>>> ListarTodosCursos(CancellationToken cancellationToken)
        {
            var cursos = await _cursoQueries.ObterTodos(cancellationToken);
            return CustomResponse(HttpStatusCode.OK, cursos);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> AdicionarCurso([FromBody] AdicionarCursoRequest request)
        {
            if (!ModelState.IsValid)
            {
                return CustomResponse(ModelState);
            }
            var command = new AdicionarCursoCommand(request.Nome, request.DescricaoConteudo, request.CargaHoraria, request.Valor, request.Disponivel);


            return CustomResponse(await _mediatorHandler.SendCommand(command));
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarCursoRequest request)
        {
            if (id != request.Id)
            {
                AdicionarErroProcessamento("O id informado não é o mesmo que foi passado no body");
                return CustomResponse();
            }
            if (!ModelState.IsValid)
            {
                return CustomResponse(ModelState);
            }

            var command = new AtualizarCursoCommand(request.Id, request.Nome, request.DescricaoConteudo, request.CargaHoraria, request.Valor, request.Disponivel);

            return CustomResponse(await _mediatorHandler.SendCommand(command));
        }

        [HttpPost("aula")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> AdicionarAula([FromBody] AdicionarAulaRequest request)
        {
            if (!ModelState.IsValid)
            {
                return CustomResponse(ModelState);
            }
            var command = new AdicionarAulaCommand(request.Titulo, request.Conteudo, request.Ordem, request.Material, request.CursoId);

            return CustomResponse(await _mediatorHandler.SendCommand(command));
        }
    }
}
