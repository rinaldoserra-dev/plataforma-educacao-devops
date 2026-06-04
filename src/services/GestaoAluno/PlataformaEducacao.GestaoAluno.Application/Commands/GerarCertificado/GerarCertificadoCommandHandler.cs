using FluentValidation.Results;
using MediatR;
using PlataformaEducacao.Core.Messages;
using PlataformaEducacao.GestaoAluno.Domain.Repositories;

namespace PlataformaEducacao.GestaoAluno.Application.Commands.GerarCertificado
{
    public class GerarCertificadoCommandHandler : CommandHandler,
        IRequestHandler<GerarCertificadoCommand, ValidationResult>
    {
        private readonly IAlunoRepository _alunoRepository;

        public GerarCertificadoCommandHandler(IAlunoRepository alunoRepository)
        {
            _alunoRepository = alunoRepository;
        }
        public async Task<ValidationResult> Handle(GerarCertificadoCommand message, CancellationToken cancellationToken)
        {
            var matricula = await _alunoRepository.ObterMatriculaComCertificadoPorId(message.MatriculaId, cancellationToken);
            matricula?.GerarCertificado();

            await _alunoRepository.GerarCertificado(matricula!.Certificado!, cancellationToken);

            return await PersistirDados(_alunoRepository.UnitOfWork);
        }
    }
}
