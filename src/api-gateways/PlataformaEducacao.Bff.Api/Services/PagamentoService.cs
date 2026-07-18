using Microsoft.Extensions.Options;
using PlataformaEducacao.Bff.Api.Extensions;
using PlataformaEducacao.Bff.Api.Models.GestaoFinanceira;
using PlataformaEducacao.Core.Communication;

namespace PlataformaEducacao.Bff.Api.Services
{
    public interface IPagamentoService
    {
        Task<ResponseResult> PagarMatricula(PagarMatriculaDTO pagamento);
        Task<ResponseResult> ObterStatus(Guid matriculaId);
        Task<ResponseResult> HealthCheck();
    }

    public class PagamentoService : Service, IPagamentoService
    {
        private readonly HttpClient _httpClient;

        public PagamentoService(HttpClient httpClient,
                                IOptions<AppServicesSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.GestaoFinanceiraUrl);
        }

        public async Task<ResponseResult> PagarMatricula(PagarMatriculaDTO pagamento)
        {
            var conteudoPagamento = ObterConteudo(pagamento);
            var response = await _httpClient.PostAsync("/api/Pagamento/pagar", conteudoPagamento);

            return await DeserializarObjetoResponse(response);
        }

        public async Task<ResponseResult> ObterStatus(Guid matriculaId)
        {
            var response = await _httpClient.GetAsync($"/api/Pagamento/{matriculaId}/status");

            return await DeserializarObjetoResponse(response);
        }

        public async Task<ResponseResult> HealthCheck()
        {
            var response = await _httpClient.GetAsync("/swagger/index.html");

            return await DeserializarObjetoResponse(response);
        }
    }
}
