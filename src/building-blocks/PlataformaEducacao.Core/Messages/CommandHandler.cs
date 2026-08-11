using FluentValidation.Results;
using PlataformaEducacao.Core.Data;

namespace PlataformaEducacao.Core.Messages
{
    public abstract class CommandHandler
    {
        private readonly ValidationResult _validationResult;

        protected CommandHandler()
        {
            _validationResult = new ValidationResult();
        }

        protected void AdicionarErro(string mensagem)
        {
            _validationResult.Errors.Add(new ValidationFailure(string.Empty, mensagem));
        }

        protected async Task<ValidationResult> PersistirDados(IUnitOfWork uow)
        {
            if (!await uow.Commit()) AdicionarErro("Houve um erro ao persistir os dados");

            return _validationResult;
        }
    }
}
