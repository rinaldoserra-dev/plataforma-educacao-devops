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

    public class MatricularAlunoCursoCommandValidation : AbstractValidator<MatricularAlunoCursoCommand>
    {
        public MatricularAlunoCursoCommandValidation()
        {
            RuleFor(a => a.AlunoId)
                .NotEmpty()
                .WithMessage("O id do aluno é obrigatório.");

            RuleFor(a => a.CursoId)
                .NotEmpty()
                .WithMessage("O id do curso é obrigatório.");

            RuleFor(a => a.NomeCurso)
                .NotEmpty()
                .WithMessage("O nome do curso é obrigatório.");

            RuleFor(c => c.Valor)
             .GreaterThan(0)
             .WithMessage("O valor do curso deve ser maior que 0.");

            RuleFor(a => a.TotalAulasCurso)
                .GreaterThan(0)
                .WithMessage("O número de aulas do curso é obrigatório.");
        }
    }
}