using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaEducacao.GestaoFinanceira.Api.Models.Requests;
using PlataformaEducacao.GestaoFinanceira.Api.Models.Response;
using PlataformaEducacao.GestaoFinanceira.Api.Services;
using PlataformaEducacao.GestaoFinanceira.Business.Models;
using PlataformaEducacao.WebApi.Core.Controllers;
using PlataformaEducacao.WebApi.Core.Usuario;
using System.Net;

namespace PlataformaEducacao.GestaoFinanceira.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagamentoController : MainController
    {
        private readonly IPagamentoService _pagamentoService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IAspNetUser _user;

        public PagamentoController(IPagamentoService pagamentoService, IServiceProvider serviceProvider, IAspNetUser user)
        {
            _pagamentoService = pagamentoService;
            _serviceProvider = serviceProvider;
            _user = user;
        }

        [HttpPost("pagar")]
        [Authorize]
        public async Task<IActionResult> PagarMatricula([FromBody] PagarMatriculaRequest dadosPagamento, CancellationToken cancellationToken)
        {
            var isAdm = _user.PossuiRole("ADMIN");
            var alunoId = _user.ObterUserId();
            if (isAdm)
            {
                alunoId = dadosPagamento.AlunoId;
            }
            else if (alunoId != dadosPagamento.AlunoId)
            {
                AdicionarErroProcessamento("Usuário não autorizado a realizar pagamento para outro aluno.");
                return CustomResponse();
            }

            if (ObterStatus(dadosPagamento.MatriculaId, cancellationToken).Result is OkObjectResult statusResult)
            {
                var status = statusResult.Value as PagamentoStatusResponse;
                if (status != null && (status.Status == "Pago" || status.Status == "Autorizado"))
                {
                    AdicionarErroProcessamento("Matrícula já está paga.");
                    return CustomResponse();
                }
            }

            var pagamento = new Pagamento
            {
                AlunoId = alunoId,
                MatriculaId = dadosPagamento.MatriculaId,
                TipoPagamento = TipoPagamento.CartaoCredito,
                Valor = dadosPagamento.Valor,
                DadosCartao = new DadosCartao(dadosPagamento.NomeCartao, dadosPagamento.NumeroCartao, dadosPagamento.ExpiracaoCartao, dadosPagamento.CvvCartao)
            };

            var result = await _pagamentoService.AutorizarPagamento(pagamento, cancellationToken);

            if (!result.ValidationResult.IsValid)
            {
                foreach (var error in result.ValidationResult.Errors)
                    AdicionarErroProcessamento(error.ErrorMessage);

                return CustomResponse();
            }

            return CustomResponse(HttpStatusCode.OK, "Pagamento autorizado com sucesso");

        }


        [HttpGet("{matriculaId:guid}/status")]
        [Authorize]
        public async Task<IActionResult> ObterStatus(Guid matriculaId, CancellationToken cancellationToken)
        {
            var usuarioId = _user.ObterUserId();
            var isAdm = _user.PossuiRole("ADMIN");

            var result = await _pagamentoService.ObterStatusPorMatricula(matriculaId, usuarioId, isAdm);

            if (result == null)
            {
                result = new PagamentoStatusResponse
                {
                    MatriculaId = matriculaId,
                    Status = "Pagamento Pendente"
                };
            }


            return CustomResponse(HttpStatusCode.OK, result);
        }


    }
}
