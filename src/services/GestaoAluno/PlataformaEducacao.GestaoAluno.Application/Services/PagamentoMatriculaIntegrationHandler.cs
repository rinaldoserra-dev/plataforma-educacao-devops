using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlataformaEducacao.Core.DomainObjects;
using PlataformaEducacao.Core.Messages.Integration;
using PlataformaEducacao.GestaoAluno.Domain.Repositories;
using PlataformaEducacao.MessageBus;

namespace PlataformaEducacao.GestaoAluno.Application.Services
{
    public class PagamentoMatriculaIntegrationHandler : BackgroundService
    {
        private readonly IMessageBus _bus;
        private readonly IServiceProvider _serviceProvider;

        public PagamentoMatriculaIntegrationHandler(IMessageBus bus, IServiceProvider serviceProvider)
        {
            _bus = bus;
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SetSubscribers();
            return Task.CompletedTask;
        }

        private void SetSubscribers()
        {
            _bus.SubscribeAsync<MatriculaPagamentoRecusadoIntegrationEvent>("MatriculaRecusada",
                async request => await RecusarMatricula(request));

            _bus.SubscribeAsync<MatriculaPagamentoRealizadoIntegrationEvent>("MatriculaConfirmada",
               async request => await FinalizarMatricula(request));
        }

        private async Task RecusarMatricula(MatriculaPagamentoRecusadoIntegrationEvent message)
        {
            using var scope = _serviceProvider.CreateScope();

            var _alunoRepository = scope.ServiceProvider.GetRequiredService<IAlunoRepository>();

            var matricula = await _alunoRepository.ObterMatriculaComAlunoPorId(message.MatriculaId, default)
                ?? throw new DomainException($"Matrícula {message.MatriculaId} não encontrada.");

            matricula.Aluno.RecusarPagamentoMatricula(matricula);

            if (await _alunoRepository.UnitOfWork.Commit() is false)
                throw new DomainException($"Problemas ao cancelar a matrícula {message.MatriculaId}");
        }

        private async Task FinalizarMatricula(MatriculaPagamentoRealizadoIntegrationEvent message)
        {
            using var scope = _serviceProvider.CreateScope();

            var _alunoRepository = scope.ServiceProvider.GetRequiredService<IAlunoRepository>();

            var matricula = await _alunoRepository.ObterMatriculaComAlunoPorId(message.MatriculaId, default)
                ?? throw new DomainException($"Matrícula {message.MatriculaId} não encontrada.");

            matricula.Aluno.ConcluirPagamentoMatricula(matricula);

            if (await _alunoRepository.UnitOfWork.Commit() is false)
                throw new DomainException($"Problemas ao ativar a matricula {message.MatriculaId}");
        }
    }
}