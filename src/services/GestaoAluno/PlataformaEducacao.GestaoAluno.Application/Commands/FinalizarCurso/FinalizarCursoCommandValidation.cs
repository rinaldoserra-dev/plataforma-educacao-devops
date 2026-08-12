using FluentValidation;
using PlataformaEducacao.Core.Messages;

namespace PlataformaEducacao.GestaoAluno.Application.Commands.FinalizarCurso
{
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
