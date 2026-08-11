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

        public async Task<ValidationResult> Handle(MatricularAlunoCursoCommand request, CancellationToken cancellationToken)
        {
            if (request.EhValido() is false) return request.ValidationResult;

            var aluno = await _alunoRepository.ObterComMatriculasPorId(request.AlunoId, cancellationToken);
            if (aluno is null)
            {
                AdicionarErro("Aluno não encontrado!");
                return ValidationResult;
            }

            var matricula = new Matricula(request.CursoId, request.NomeCurso, request.TotalAulasCurso, request.Valor);
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
