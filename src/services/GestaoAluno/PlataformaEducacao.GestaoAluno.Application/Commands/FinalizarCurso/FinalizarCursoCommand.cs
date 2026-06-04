using FluentValidation;
using PlataformaEducacao.Core.Messages;

namespace PlataformaEducacao.GestaoAluno.Application.Commands.FinalizarCurso
{
    public class FinalizarCursoCommand : Command
    {
        public FinalizarCursoCommand(Guid matriculaId)
        {
            MatriculaId = matriculaId;
        }

        public Guid MatriculaId { get; private set; }

        public override bool EhValido()
        {
            ValidationResult = new FinalizarCursoCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }

    public class FinalizarCursoCommandValidation : AbstractValidator<FinalizarCursoCommand>
    {
        public FinalizarCursoCommandValidation()
        {
            RuleFor(c => c.MatriculaId)
                .NotEqual(Guid.Empty)
                .WithMessage("Id da matrícula inválido.");
        }
    }
}