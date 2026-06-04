using Microsoft.AspNetCore.Mvc;
using PlataformaEducacao.Bff.Api.Models.Request.Identidade;
using PlataformaEducacao.Bff.Api.Services;

namespace PlataformaEducacao.Bff.Api.Controllers
{
    public class IdentidadeController : BaseController
    {
        private readonly IIdentidadeService _identidadeService;

        public IdentidadeController(IIdentidadeService identidadeService)
        {
            _identidadeService = identidadeService;
        }

        [HttpPost]
        [Route("identidade/autenticar")]
        public async Task<IActionResult> Autenticar(LoginRequest login)
        {
            var resposta = await _identidadeService.Login(login);

            return CustomResponse(resposta);
        }

        [HttpPost]
        [Route("identidade/novo-aluno")]
        public async Task<IActionResult> NovoAluno(RegistroAlunoRequest registroAluno)
        {
            var resposta = await _identidadeService.RegistrarAluno(registroAluno);

            return CustomResponse(resposta);
        }

        [HttpPost]
        [Route("identidade/refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest refreshToken)
        {
            var resposta = await _identidadeService.RefreshToken(refreshToken);

            return CustomResponse(resposta);
        }
    }
}
