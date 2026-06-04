using PlataformaEducacao.Core.DomainObjects;


namespace PlataformaEducacao.GestaoFinanceira.Business.Models
{
    public class Pagamento : Entity, IAggregateRoot
    {
        public Pagamento()
        {
            Transacoes = new List<Transacao>();
        }

        public Guid AlunoId { get; set; }

        public Guid MatriculaId { get; set; }

        public TipoPagamento TipoPagamento { get; set; }
        public decimal Valor { get; set; }

        public DadosCartao DadosCartao { get; set; } = null!;

        // EF Relation
        public ICollection<Transacao> Transacoes { get; set; }

        public void AdicionarTransacao(Transacao transacao)
        {
            Transacoes.Add(transacao);
        }
    }
}
