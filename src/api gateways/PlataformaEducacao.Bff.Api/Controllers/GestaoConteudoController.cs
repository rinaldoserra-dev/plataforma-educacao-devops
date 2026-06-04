using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaEducacao.Bff.Api.Models.Request.GestaoConteudo;
using PlataformaEducacao.Bff.Api.Services;

namespace PlataformaEducacao.Bff.Api.Controllers
{
    [Route("cursos")]
    [Authorize]
    public class GestaoConteudoController : BaseController
    {
        private readonly ICursosService _cursosService;

        public GestaoConteudoController(ICursosService cursosService)
        {
            _cursosService = cursosService;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> AdicionarCurso([FromBody] AdicionarCursoRequest cursoRequest)
        {
            var resposta = await _cursosService.AdicionarCurso(cursoRequest);

            return CustomResponse(resposta);
        }
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarCursoRequest cursoRequest)
        {
            var resposta = await _cursosService.AtualizarCurso(id, cursoRequest);

            return CustomResponse(resposta);
        }

        [HttpPost]
        [Route("aula")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> AdicionarAula([FromBody] AdicionarAulaRequest aulaRequest)
        {
            var resposta = await _cursosService.AdicionarAula(aulaRequest);

            return CustomResponse(resposta);
        }

        [AllowAnonymous]
        [HttpGet("listar-cursos-disponiveis")]
        public async Task<ActionResult> ListarCursosDisponiveisParaMatricula()
        {
            var cursos = await _cursosService.ObterCursosDisponiveisComAula();

            return CustomResponse(cursos);
        }

        [HttpGet("{cursoId:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult> ObterDetalhesCurso(Guid cursoId)
        {
            var curso = await _cursosService.ObterCursoComAulasPorCursoId(cursoId);

            return CustomResponse(curso);
        }

        [HttpGet("listar-todos-cursos")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> ListarTodosCursos()
        {
            var cursos = await _cursosService.ObterTodos();

            return CustomResponse(cursos);
        }
    }
}
