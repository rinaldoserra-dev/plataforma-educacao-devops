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

    internal class FakePagamentoService : IPagamentoService
    {
        public Task<CoreResponseResult> HealthCheck()
            => Ok(new { });

        public Task<CoreResponseResult> ObterStatus(Guid matriculaId)
            => Ok(new { MatriculaId = matriculaId, Status = "Autorizado" });

        public Task<CoreResponseResult> PagarMatricula(PlataformaEducacao.Bff.Api.Models.GestaoFinanceira.PagarMatriculaDTO pagamento)
            => Ok(new { pagamento.MatriculaId, pagamento.Valor });

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
