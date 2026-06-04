using FluentValidation.Results;
using MediatR;
using PlataformaEducacao.Core.Messages;
using PlataformaEducacao.GestaoConteudo.Domain;
using PlataformaEducacao.GestaoConteudo.Domain.ValueObjects;

namespace PlataformaEducacao.GestaoConteudo.Application.Commands
{
    public class CursoCommandHandler : CommandHandler,
         IRequestHandler<AdicionarCursoCommand, ValidationResult>,
        IRequestHandler<AtualizarCursoCommand, ValidationResult>,
        IRequestHandler<AdicionarAulaCommand, ValidationResult>
    {
        private readonly ICursoRepository _cursoRepository;

        public CursoCommandHandler(ICursoRepository cursoRepository)
        {
            _cursoRepository = cursoRepository;
        }

        public async Task<ValidationResult> Handle(AdicionarCursoCommand message, CancellationToken cancellationToken)
        {
            if (!message.EhValido()) return message.ValidationResult;

            var curso = await _cursoRepository.ObterPorNome(message.Nome, cancellationToken);

            if (curso is not null)
            {
                AdicionarErro("Já possui curso com esse nome!");
                return ValidationResult;
            }
            var conteudoProgramatico = new ConteudoProgramatico(message.DescricaoConteudo, message.CargaHoraria);

            curso = new Curso(message.Nome, conteudoProgramatico, message.Valor, message.Disponivel);

            await _cursoRepository.Inserir(curso, cancellationToken);

            return await PersistirDados(_cursoRepository.UnitOfWork);
        }

        public async Task<ValidationResult> Handle(AtualizarCursoCommand message, CancellationToken cancellationToken)
        {
            if (!message.EhValido()) return message.ValidationResult;

            var cursoAtualizar = await _cursoRepository.ObterPorId(message.CursoId, cancellationToken);

            if (cursoAtualizar is null)
            {
                AdicionarErro("Curso não encontrado!");
                return ValidationResult;
            }

            var curso = await _cursoRepository.ObterPorNome(message.Nome, cancellationToken);
            if (curso is not null && curso.Id != cursoAtualizar.Id)
            {
                AdicionarErro("O nome do curso já existe!");
                return ValidationResult;
            }

            cursoAtualizar.AtualizarNome(message.Nome);
            cursoAtualizar.AtualizarValor(message.Valor);
            cursoAtualizar.AtualizarConteudoProgramatico(new ConteudoProgramatico(message.DescricaoConteudo, message.CargaHoraria));
            if (message.Disponivel)
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

        public async Task<ValidationResult> Handle(AdicionarAulaCommand message, CancellationToken cancellationToken)
        {
            if (!message.EhValido()) return message.ValidationResult;

            var curso = await _cursoRepository.ObterComAulasPorId(message.CursoId, cancellationToken);

            if (curso is null)
            {
                AdicionarErro("Curso não encontrado!");
                return ValidationResult;
            }
            var aula = new Aula(message.Titulo, message.Conteudo, message.Ordem, message.Material);
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
