using PlataformaEducacao.Core.Messages;

namespace PlataformaEducacao.GestaoAluno.Domain.Events
{
    public class CursoFinalizadoEvent : Event
    {
        public Guid MatriculaId { get; private set; }

        public CursoFinalizadoEvent(Guid matriculaId)
        {
            AggregateId = matriculaId;
            MatriculaId = matriculaId;
        }
    }
}
