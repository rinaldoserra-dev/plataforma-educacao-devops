using FluentValidation.Results;
using MediatR;
using PlataformaEducacao.Core.Messages;
using PlataformaEducacao.GestaoAluno.Domain;
using PlataformaEducacao.GestaoAluno.Domain.Repositories;

namespace PlataformaEducacao.GestaoAluno.Application.Commands.MatricularAlunoCurso
{
    public class MatricularAlunoCursoCommandHandler : CommandHandler,
        IRequestHandler<MatricularAlunoCursoCommand, ValidationResult>
    {
        private readonly IAlunoRepository _alunoRepository;

        public MatricularAlunoCursoCommandHandler(IAlunoRepository alunoRepository)
        {
            _alunoRepository = alunoRepository;
        }

        public async Task<ValidationResult> Handle(MatricularAlunoCursoCommand message, CancellationToken cancellationToken)
        {
            if (message.EhValido() is false) return message.ValidationResult;

            var aluno = await _alunoRepository.ObterComMatriculasPorId(message.AlunoId, cancellationToken);
            if (aluno is null)
            {
                AdicionarErro("Aluno não encontrado!");
                return ValidationResult;
            }

            var matricula = new Matricula(message.CursoId, message.NomeCurso, message.TotalAulasCurso, message.Valor);
            if (aluno.MatriculaExistente(matricula))
            {
                AdicionarErro("Aluno já matriculado no curso!");
                return ValidationResult;
            }

            aluno.RealizarMatricula(matricula);

            await _alunoRepository.RealizarMatricula(matricula, cancellationToken);

            return await PersistirDados(_alunoRepository.UnitOfWork);
        }
    }
}