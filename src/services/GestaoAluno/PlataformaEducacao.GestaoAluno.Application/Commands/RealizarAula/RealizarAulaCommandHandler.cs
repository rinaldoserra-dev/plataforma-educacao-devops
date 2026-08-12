using FluentValidation.Results;
using MediatR;
using PlataformaEducacao.Core.Messages;
using PlataformaEducacao.GestaoAluno.Domain;
using PlataformaEducacao.GestaoAluno.Domain.Repositories;

namespace PlataformaEducacao.GestaoAluno.Application.Commands.RealizarAula
{
    public class RealizarAulaCommandHandler : CommandHandler,
        IRequestHandler<RealizarAulaCommand, ValidationResult>
    {
        private readonly IAlunoRepository _alunoRepository;

        public RealizarAulaCommandHandler(IAlunoRepository alunoRepository)
        {
            _alunoRepository = alunoRepository;
        }

        public async Task<ValidationResult> Handle(RealizarAulaCommand request, CancellationToken cancellationToken)
        {
            if (request.EhValido() is false) return request.ValidationResult;

            var matricula = await _alunoRepository.ObterMatriculaComProgressoAulasPorId(request.MatriculaId, cancellationToken);
            if (matricula is null)
            {
                AdicionarErro("Matrícula não encontrada.");
                return ValidationResult;
            }

            if (!matricula.EstaAtiva())
            {
                AdicionarErro("Matrícula pendente de pagamento.");
                return ValidationResult;
            }

            if (matricula.CursoId != request.CursoId)
            {
                AdicionarErro("Essa aula não faz parte do curso dessa matrícula.");
                return ValidationResult;
            }

            var progressoAula = new ProgressoAula(request.AulaId);
            if (matricula.AulaRealizada(progressoAula))
            {
                AdicionarErro("Aula já realizada.");
                return ValidationResult;
            }

            matricula.RegistrarAula(progressoAula);

            await _alunoRepository.AtualizarProgressoAula(progressoAula, cancellationToken);
            await _alunoRepository.AtualizarMatricula(matricula, cancellationToken);

            return await PersistirDados(_alunoRepository.UnitOfWork);
        }
    }
}
