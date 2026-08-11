using FluentValidation.Results;
using PlataformaEducacao.Core.Data;

namespace PlataformaEducacao.Core.Messages
{
    public abstract class CommandHandler
    {
        protected CommandHandler()
        {
            ValidationResult = new ValidationResult();
        }

        // Expor o ValidationResult para classes derivadas
        protected ValidationResult ValidationResult { get; }

        protected void AdicionarErro(string mensagem)
        {
            ValidationResult.Errors.Add(new ValidationFailure(string.Empty, mensagem));
        }

        protected async Task<ValidationResult> PersistirDados(IUnitOfWork uow)
        {
            if (!await uow.Commit()) AdicionarErro("Houve um erro ao persistir os dados");

            return ValidationResult;
        }
    }
}
