using FluentValidation;
using PlataformaEducacao.Core.Messages;

namespace PlataformaEducacao.GestaoConteudo.Application.Commands
{
    public class AdicionarAulaCommand : Command
    {
        public AdicionarAulaCommand(string titulo, string conteudo, int ordem, string? material, Guid cursoId)
        {
            Titulo = titulo;
            Conteudo = conteudo;
            Ordem = ordem;
            Material = material;
            CursoId = cursoId;
        }
        public Guid CursoId { get; private set; }
        public string Titulo { get; private set; }
        public string Conteudo { get; private set; }
        public int Ordem { get; private set; }
        public string? Material { get; set; }

        public override bool EhValido()
        {
            ValidationResult = new AdicionarAulaCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }

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
