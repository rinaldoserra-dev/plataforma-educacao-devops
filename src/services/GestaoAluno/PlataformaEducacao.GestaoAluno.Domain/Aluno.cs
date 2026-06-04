using PlataformaEducacao.Core;
using PlataformaEducacao.Core.DomainObjects;

namespace PlataformaEducacao.GestaoAluno.Domain
{
    public class Aluno : Entity, IAggregateRoot
    {
        public string Nome { get; private set; } = null!;
        public Email Email { get; private set; } = null!;

        private readonly List<Matricula> _matriculas;
        public IReadOnlyCollection<Matricula> Matriculas => _matriculas;

        protected Aluno()
        {
            _matriculas = [];
        }

        public Aluno(Guid alunoId, string nome, string email)
        {
            Id = alunoId;
            Nome = nome;
            Email = new Email(email);
            _matriculas = [];

            Validar();
        }

        protected void Validar()
        {
            Validacoes.ValidarSeVazio(Id, "O id do aluno é obrigatório.");
            Validacoes.ValidarSeVazio(Nome, "O nome do aluno é obrigatório.");
        }

        public void RealizarMatricula(Matricula matricula)
        {
            // Verifica se o aluno já possui uma matrícula para o mesmo curso
            if (MatriculaExistente(matricula))
                throw new DomainException($"Aluno {Id} já matriculado no curso {matricula.CursoId}.");

            // Associa a matrícula ao aluno
            matricula.AssociarAluno(Id);

            // Adiciona a matrícula à lista de matrículas do aluno
            _matriculas.Add(matricula);
        }

        public bool MatriculaExistente(Matricula matricula)
        {
            return _matriculas.Any(p => p.CursoId == matricula.CursoId);
        }

        public void RecusarPagamentoMatricula(Matricula matricula)
        {
            if (MatriculaExistente(matricula) is false)
                throw new DomainException($"Matricula {matricula.Id} não encontrada para o aluno {Id}.");

            matricula.Recusar();
        }

        public void ConcluirPagamentoMatricula(Matricula matricula)
        {
            if (MatriculaExistente(matricula) is false)
                throw new DomainException($"Matricula {matricula.Id} não encontrada para o aluno {Id}.");

            matricula.Ativar();
        }
    }
}