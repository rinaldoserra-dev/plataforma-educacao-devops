using FluentValidation.Results;
using MediatR;
using PlataformaEducacao.Core.Messages;
using PlataformaEducacao.GestaoConteudo.Domain;
using PlataformaEducacao.GestaoConteudo.Domain.ValueObjects;

namespace PlataformaEducacao.GestaoConteudo.Application.Commands
{
    public class CursoCommandHandler : CommandHandler, IRequestHandler<AdicionarCursoCommand, ValidationResult>,
        IRequestHandler<AtualizarCursoCommand, ValidationResult>, IRequestHandler<AdicionarAulaCommand, ValidationResult>
    {
        private readonly ICursoRepository _cursoRepository;

        public CursoCommandHandler(ICursoRepository cursoRepository)
        {
            _cursoRepository = cursoRepository;
        }

        public async Task<ValidationResult> Handle(AdicionarCursoCommand request, CancellationToken cancellationToken)
        {
            if (!request.EhValido()) return request.ValidationResult;

            var curso = await _cursoRepository.ObterPorNome(request.Nome, cancellationToken);

            if (curso is not null)
            {
                AdicionarErro("Já possui curso com esse nome!");
                return ValidationResult;
            }

            var conteudoProgramatico = new ConteudoProgramatico(request.DescricaoConteudo, request.CargaHoraria);

            curso = new Curso(request.Nome, conteudoProgramatico, request.Valor, request.Disponivel);

            await _cursoRepository.Inserir(curso, cancellationToken);

            return await PersistirDados(_cursoRepository.UnitOfWork);
        }

        public async Task<ValidationResult> Handle(AtualizarCursoCommand request, CancellationToken cancellationToken)
        {
            if (!request.EhValido()) return request.ValidationResult;

            var cursoAtualizar = await _cursoRepository.ObterPorId(request.CursoId, cancellationToken);

            if (cursoAtualizar is null)
            {
                AdicionarErro("Curso não encontrado!");
                return ValidationResult;
            }

            var curso = await _cursoRepository.ObterPorNome(request.Nome, cancellationToken);
            if (curso is not null && curso.Id != cursoAtualizar.Id)
            {
                AdicionarErro("O nome do curso já existe!");
                return ValidationResult;
            }

            cursoAtualizar.AtualizarNome(request.Nome);
            cursoAtualizar.AtualizarValor(request.Valor);
            cursoAtualizar.AtualizarConteudoProgramatico(new ConteudoProgramatico(request.DescricaoConteudo, request.CargaHoraria));
            if (request.Disponivel)
            {
                cursoAtualizar.TornarDisponivel();
            }
            else
            {
                cursoAtualizar.TornarIndisponivel();
            }

            await _cursoRepository.Atualizar(cursoAtualizar, cancellationToken);

            return await PersistirDados(_cursoRepository.UnitOfWork);
        }

        public async Task<ValidationResult> Handle(AdicionarAulaCommand request, CancellationToken cancellationToken)
        {
            if (!request.EhValido()) return request.ValidationResult;

            var curso = await _cursoRepository.ObterComAulasPorId(request.CursoId, cancellationToken);

            if (curso is null)
            {
                AdicionarErro("Curso não encontrado!");
                return ValidationResult;
            }

            var aula = new Aula(request.Titulo, request.Conteudo, request.Ordem, request.Material);
            if (curso.AulaExistente(aula))
            {
                AdicionarErro("O curso já possui uma aula com esse titulo!");
                return ValidationResult;
            }

            curso.AdicionarAula(aula);

            await _cursoRepository.InserirAula(aula, cancellationToken);

            return await PersistirDados(_cursoRepository.UnitOfWork);
        }
    }
}
