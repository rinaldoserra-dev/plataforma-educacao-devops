using FluentValidation;
using PlataformaEducacao.Core.Messages;

namespace PlataformaEducacao.GestaoAluno.Application.Commands.MatricularAlunoCurso
{
    public class MatricularAlunoCursoCommand : Command
    {
        public Guid CursoId { get; private set; }

        public Guid AlunoId { get; private set; }

        public string NomeCurso { get; private set; }

        public decimal Valor { get; private set; }

        public int TotalAulasCurso { get; private set; }

        public MatricularAlunoCursoCommand(Guid cursoId, Guid alunoId, string nomeCurso, int totalAulasCurso, decimal valor)
        {
            CursoId = cursoId;
            AlunoId = alunoId;
            NomeCurso = nomeCurso;
            TotalAulasCurso = totalAulasCurso;
            Valor = valor;
        }

        public override bool EhValido()
        {
            ValidationResult = new MatricularAlunoCursoCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }

        public void VincularAluno(Guid guid)
        {
            AlunoId = guid;
        }
    }
}
