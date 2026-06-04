using FluentValidation;
using PlataformaEducacao.Core.Messages;

namespace PlataformaEducacao.GestaoConteudo.Application.Commands
{
    public class AtualizarCursoCommand : Command
    {
        public AtualizarCursoCommand(Guid cursoId, string nome, string descricaoConteudo, int cargaHoraria, decimal valor, bool disponivel)
        {
            CursoId = cursoId;
            Nome = nome;
            DescricaoConteudo = descricaoConteudo;
            CargaHoraria = cargaHoraria;
            Valor = valor;
            Disponivel = disponivel;
        }
        public Guid CursoId { get; private set; }
        public string Nome { get; private set; }
        public string DescricaoConteudo { get; private set; }
        public int CargaHoraria { get; private set; }
        public decimal Valor { get; private set; }
        public bool Disponivel { get; private set; }

        public override bool EhValido()
        {
            ValidationResult = new AtualizarCursoCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }

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
