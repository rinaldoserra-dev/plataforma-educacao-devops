namespace PlataformaEducacao.Core.Messages.Integration
{
    public class MatriculaPagamentoRecusadoIntegrationEvent : IntegrationEvent
    {
        public Guid MatriculaId { get; private set; }

        public MatriculaPagamentoRecusadoIntegrationEvent(Guid matriculaId)
        {
            AggregateId = matriculaId;
            MatriculaId = matriculaId;
        }
    }
}
