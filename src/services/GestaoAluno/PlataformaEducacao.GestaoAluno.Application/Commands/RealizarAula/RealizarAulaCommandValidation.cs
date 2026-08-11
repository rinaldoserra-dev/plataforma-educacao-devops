using FluentValidation;
using PlataformaEducacao.Core.Messages;

namespace PlataformaEducacao.GestaoAluno.Application.Commands.RealizarAula
{
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
