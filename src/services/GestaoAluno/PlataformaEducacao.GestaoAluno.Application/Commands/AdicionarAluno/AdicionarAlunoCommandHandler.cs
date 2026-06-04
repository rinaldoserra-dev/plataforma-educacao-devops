using FluentValidation.Results;
using MediatR;
using PlataformaEducacao.Core.Messages;
using PlataformaEducacao.GestaoAluno.Domain;
using PlataformaEducacao.GestaoAluno.Domain.Repositories;

namespace PlataformaEducacao.GestaoAluno.Application.Commands.AdicionarAluno
{
    public class AdicionarAlunoCommandHandler : CommandHandler,
        IRequestHandler<AdicionarAlunoCommand, ValidationResult>
    {
        private readonly IAlunoRepository _alunoRepository;

        public AdicionarAlunoCommandHandler(IAlunoRepository alunoRepository)
        {
            _alunoRepository = alunoRepository;
        }

        public async Task<ValidationResult> Handle(AdicionarAlunoCommand message, CancellationToken cancellationToken)
        {
            if (!message.EhValido()) return message.ValidationResult;

            var aluno = new Aluno(message.UsuarioId, message.Nome, message.Email);

            await _alunoRepository.Inserir(aluno, cancellationToken);

            return await PersistirDados(_alunoRepository.UnitOfWork);
        }
    }
}
