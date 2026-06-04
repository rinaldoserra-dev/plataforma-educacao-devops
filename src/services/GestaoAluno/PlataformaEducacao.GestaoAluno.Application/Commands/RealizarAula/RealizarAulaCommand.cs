using FluentValidation;
using PlataformaEducacao.Core.Messages;

namespace PlataformaEducacao.GestaoAluno.Application.Commands.RealizarAula
{
    public class RealizarAulaCommand : Command
    {
        public Guid MatriculaId { get; private set; }
        public Guid CursoId { get; private set; }
        public Guid AulaId { get; private set; }

        public RealizarAulaCommand(Guid matriculaId, Guid cursoId, Guid aulaId)
        {
            MatriculaId = matriculaId;
            CursoId = cursoId;
            AulaId = aulaId;
        }

        public override bool EhValido()
        {
            ValidationResult = new RealizarAulaCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }

    public class RealizarAulaCommandValidation : AbstractValidator<RealizarAulaCommand>
    {
        public RealizarAulaCommandValidation()
        {
            RuleFor(c => c.AulaId)
                .NotEqual(Guid.Empty)
                .WithMessage("Id da aula inválido.");

            RuleFor(c => c.MatriculaId)
                .NotEqual(Guid.Empty)
                .WithMessage("Id da matrícula inválido.");

            RuleFor(c => c.CursoId)
                .NotEqual(Guid.Empty)
                .WithMessage("Id do curso inválido.");
        }
    }
}