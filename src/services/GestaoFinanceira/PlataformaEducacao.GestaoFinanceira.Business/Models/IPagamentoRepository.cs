using PlataformaEducacao.Core.Data;

namespace PlataformaEducacao.GestaoFinanceira.Business.Models
{
    public interface IPagamentoRepository : IRepository<Pagamento>
    {
        void AdicionarPagamento(Pagamento pagamento);
        void AdicionarTransacao(Transacao transacao);
        Task<Pagamento> ObterPagamentoPorMatriculaId(Guid matriculaId);
        Task<Pagamento?> ObterPagamentoPorMatriculaId(Guid matriculaId, Guid usuarioId, bool isAdm);
        Task<IEnumerable<Transacao>> ObterTransacoesPorMatriculaId(Guid matriculaId);
    }
}
