using PlataformaEducacao.Core.Messages;

namespace PlataformaEducacao.GestaoAluno.Domain.Events
{
    public class MatriculaAtivadaEvent : Event
    {
        public Guid MatriculaId { get; private set; }

        public MatriculaAtivadaEvent(Guid matriculaId)
        {
            AggregateId = matriculaId;
            MatriculaId = matriculaId;

        }
    }
}
