using FluentValidation;
using PlataformaEducacao.Core.Messages;

namespace PlataformaEducacao.GestaoConteudo.Application.Commands
{
    public class AdicionarAulaCommandValidation : AbstractValidator<AdicionarAulaCommand>
    {
        public AdicionarAulaCommandValidation()
        {
            RuleFor(c => c.Titulo)
                .NotEmpty()
                .WithMessage("Título da aula é obrigatório.")
                .MaximumLength(255)
                .WithMessage("Título da aula deve ter no máximo 255 caracteres.");

            RuleFor(c => c.Conteudo)
                .NotEmpty()
                .WithMessage("O conteudo é obrigatório.")
                .MaximumLength(1000)
                .WithMessage("O conteudo deve ter no máximo 1000 caracteres.");

            RuleFor(c => c.Ordem)
             .GreaterThan(0)
             .WithMessage("A ordem da aula deve ser maior que 0.");

            RuleFor(c => c.CursoId)
               .NotEmpty()
               .WithMessage("Curso é obrigatório.");
        }
    }
}
