using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaEducacao.Bff.Api.Models.GestaoFinanceira;
using PlataformaEducacao.Bff.Api.Services;

namespace PlataformaEducacao.Bff.Api.Controllers
{
    [Authorize]
    [Route("pagamentos")]
    public class PagamentosController(IPagamentoService pagamentoService) : BaseController
    {
        private readonly IPagamentoService _pagamentoService = pagamentoService;

        [HttpPost("matriculas/pagar")]
        public async Task<IActionResult> PagarMatricula(PagarMatriculaDTO pagamento)
        {
            if (ModelState.IsValid is false)
                return CustomResponse(ModelState);

            var resposta = await _pagamentoService.PagarMatricula(pagamento);

            return CustomResponse(resposta);
        }

        [HttpGet("matriculas/{matriculaId:guid}/status")]
        public async Task<IActionResult> ObterStatus(Guid matriculaId)
        {
            var resposta = await _pagamentoService.ObterStatus(matriculaId);

            return CustomResponse(resposta);
        }
    }
}
