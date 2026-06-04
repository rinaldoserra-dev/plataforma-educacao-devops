namespace PlataformaEducacao.Core.Messages.Integration
{
    public class IniciaPagamentoIntegrationEvent : IntegrationEvent
    {
        public IniciaPagamentoIntegrationEvent(Guid matriculaId, Guid alunoId, decimal valor, int tipoPagamento, string nomeCartao, string numeroCartao, string expiracaoCartao, string cvvCartao)
        {
            AggregateId = matriculaId;
            MatriculaId = matriculaId;
            AlunoId = alunoId;
            Valor = valor;
            TipoPagamento = tipoPagamento;
            NomeCartao = nomeCartao;
            NumeroCartao = numeroCartao;
            ExpiracaoCartao = expiracaoCartao;
            CvvCartao = cvvCartao;
        }

        public Guid MatriculaId { get; private set; }
        public Guid AlunoId { get; private set; }
        public decimal Valor { get; private set; }
        public int TipoPagamento { get; set; }
        public string NomeCartao { get; private set; }
        public string NumeroCartao { get; private set; }
        public string ExpiracaoCartao { get; private set; }
        public string CvvCartao { get; private set; }
    }
}
