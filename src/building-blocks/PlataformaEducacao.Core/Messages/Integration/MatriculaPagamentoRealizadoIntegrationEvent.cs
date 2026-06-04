namespace PlataformaEducacao.Core.Messages.Integration
{
    public class MatriculaPagamentoRealizadoIntegrationEvent : IntegrationEvent
    {
        public Guid MatriculaId { get; private set; }

        public MatriculaPagamentoRealizadoIntegrationEvent(Guid matriculaId)
        {
            AggregateId = matriculaId;
            MatriculaId = matriculaId;
        }
    }
}
