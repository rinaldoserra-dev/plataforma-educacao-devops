using FluentValidation;
using PlataformaEducacao.Core.Messages;

namespace PlataformaEducacao.GestaoConteudo.Application.Commands
{

    public class AtualizarCursoCommandValidation : AbstractValidator<AtualizarCursoCommand>
    {
        public AtualizarCursoCommandValidation()
        {
            RuleFor(c => c.CursoId)
                .NotEmpty()
                .WithMessage("Id do curso é obrigatório.");

            RuleFor(c => c.Nome)
                .NotEmpty()
                .WithMessage("Nome do curso é obrigatório.")
                .MaximumLength(255)
                .WithMessage("Nome do curso deve ter no máximo 255 caracteres.");

            RuleFor(c => c.DescricaoConteudo)
                .NotEmpty()
                .WithMessage("A descrição do conteudo programático é obrigatória.")
                .MaximumLength(255)
                .WithMessage("A descrição do conteudo programático deve ter no máximo 1000 caracteres.");

            RuleFor(c => c.CargaHoraria)
             .GreaterThan(0)
             .WithMessage("A carga horária do curso deve ser maior que 0.");

            RuleFor(c => c.Valor)
             .GreaterThan(0)
             .WithMessage("O valor do curso deve ser maior que 0.");
        }
    }
}
