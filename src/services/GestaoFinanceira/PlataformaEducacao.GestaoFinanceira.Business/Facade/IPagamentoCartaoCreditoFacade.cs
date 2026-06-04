using PlataformaEducacao.GestaoFinanceira.Business.Models;

namespace PlataformaEducacao.GestaoFinanceira.Business.Facade
{
    public interface IPagamentoCartaoCreditoFacade
    {
        Task<Transacao> AutorizarPagamento(Pagamento pagamento);
        Task<Transacao> CapturarPagamento(Transacao transacao);
        Task<Transacao> CancelarAutorizacao(Transacao transacao);
    }
}
