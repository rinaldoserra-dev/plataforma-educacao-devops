using FluentValidation;
using PlataformaEducacao.Core.Messages;

namespace PlataformaEducacao.GestaoAluno.Application.Commands.AdicionarAluno
{
    public class AdicionarAlunoCommandValidation : AbstractValidator<AdicionarAlunoCommand>
    {
        public AdicionarAlunoCommandValidation()
        {
            RuleFor(a => a.UsuarioId)
                .NotEmpty()
                .WithMessage("O id do usuário é obrigatório.");

            RuleFor(a => a.Nome)
                .NotEmpty()
                .WithMessage("O nome do aluno é obrigatório.");

            RuleFor(a => a.Email)
               .NotEmpty()
               .WithMessage("O e-mail do aluno é obrigatório.");
        }
    }
}
