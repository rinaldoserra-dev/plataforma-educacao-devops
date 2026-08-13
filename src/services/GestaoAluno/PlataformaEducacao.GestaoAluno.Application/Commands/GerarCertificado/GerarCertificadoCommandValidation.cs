using FluentValidation;
using PlataformaEducacao.Core.Messages;

namespace PlataformaEducacao.GestaoAluno.Application.Commands.GerarCertificado
{
    public class GerarCertificadoCommandValidation : AbstractValidator<GerarCertificadoCommand>
    {
        public GerarCertificadoCommandValidation()
        {
            RuleFor(c => c.MatriculaId)
                .NotEqual(Guid.Empty)
                .WithMessage("Id da matrícula inválido.");
        }
    }
}
