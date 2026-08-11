using FluentValidation;
using PlataformaEducacao.Core.Messages;

namespace PlataformaEducacao.GestaoAluno.Application.Commands.MatricularAlunoCurso
{
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
