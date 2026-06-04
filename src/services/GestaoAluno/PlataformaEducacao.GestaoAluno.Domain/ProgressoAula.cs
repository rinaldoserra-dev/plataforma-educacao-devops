using PlataformaEducacao.Core;
using PlataformaEducacao.Core.DomainObjects;

namespace PlataformaEducacao.GestaoAluno.Domain
{
    public class ProgressoAula : Entity
    {
        public Guid MatriculaId { get; private set; }
        public Guid AulaId { get; private set; }
        public DateTime DataConclusao { get; private set; }
        public Matricula Matricula { get; private set; } = null!;

        protected ProgressoAula() { }

        public ProgressoAula(Guid aulaId)
        {
            AulaId = aulaId;
            DataConclusao = DateTime.Now;

            Validacoes.ValidarSeVazio(AulaId, "A aula não pode ser vazia.");
        }

        public void AssociarMatricula(Guid matriculaId)
        {
            MatriculaId = matriculaId;
            Validacoes.ValidarSeVazio(MatriculaId, "A matrícula não pode ser vazia.");
        }
    }
}