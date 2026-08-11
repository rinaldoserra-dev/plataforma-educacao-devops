using FluentValidation;
using PlataformaEducacao.Core.Messages;

namespace PlataformaEducacao.GestaoAluno.Application.Commands.AdicionarAluno
{
    public class AdicionarAlunoCommand : Command
    {
        public AdicionarAlunoCommand(Guid usuarioId, string nome, string email)
        {
            UsuarioId = usuarioId;
            Nome = nome;
            Email = email;
        }

        public Guid UsuarioId { get; private set; }

        public string Nome { get; private set; }

        public string Email { get; private set; }

        public override bool EhValido()
        {
            ValidationResult = new AdicionarAlunoCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
