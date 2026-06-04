using Microsoft.Extensions.DependencyInjection;
using Moq;
using PlataformaEducacao.Core.Data;
using PlataformaEducacao.Core.Messages.Integration;
using PlataformaEducacao.GestaoAluno.Application.Services;
using PlataformaEducacao.GestaoAluno.Domain;
using PlataformaEducacao.GestaoAluno.Domain.Repositories;
using PlataformaEducacao.MessageBus;

namespace PlataformaEducacao.GestaoAluno.Application.Tests.Services
{
    public class PagamentoMatriculaIntegrationHandlerTest
    {
        Matricula? _matricula;
        readonly Mock<IAlunoRepository>? _alunoRepositoryMock = new();
        readonly Mock<IUnitOfWork>? _uowMock = new();
        readonly Mock<IServiceProvider>? _rootServiceProviderMock = new();
        readonly Mock<IMessageBus> _messageBusMock = new();

        [Fact(DisplayName = "Recusar matrícula ao receber evento deve recusar e commitar")]
        [Trait("Categoria", "Gestão Aluno - Application - Services - PagamentoMatriculaIntegrationHandler")]
        public async Task RecusarMatricula_EventoRecebido_DeveRecusarEComitar()
        {
            // Arrange
            CriarMatriculaTeste();
            ConfigurarAlunoRepositoryMock();
            ConfigurarUnitOfWorkMock();
            ConfigurarServiceProviderMock();

            Func<MatriculaPagamentoRecusadoIntegrationEvent, Task>? acaoRecusar = null;

            _messageBusMock.Setup(b => b.SubscribeAsync<MatriculaPagamentoRecusadoIntegrationEvent>(
                It.IsAny<string>(), It.IsAny<Func<MatriculaPagamentoRecusadoIntegrationEvent, Task>>()))
                .Callback<string, Func<MatriculaPagamentoRecusadoIntegrationEvent, Task>>((_, f) => acaoRecusar = f);

            _messageBusMock.Setup(b => b.SubscribeAsync<MatriculaPagamentoRealizadoIntegrationEvent>(
                It.IsAny<string>(), It.IsAny<Func<MatriculaPagamentoRealizadoIntegrationEvent, Task>>()));

            var handler = new PagamentoMatriculaIntegrationHandler(_messageBusMock.Object, _rootServiceProviderMock!.Object);

            // Act
            await handler.StartAsync(CancellationToken.None);
            await acaoRecusar!(new MatriculaPagamentoRecusadoIntegrationEvent(_matricula!.Id));

            // Assert
            _alunoRepositoryMock!.Verify(r => r.ObterMatriculaComAlunoPorId(_matricula.Id, It.IsAny<CancellationToken>()), Times.Once);
            _uowMock!.Verify(u => u.Commit(), Times.Once);
            Assert.Equal(SituacaoMatricula.PendentePagamento, _matricula.SituacaoMatricula);
        }

        [Fact(DisplayName = "Finalizar matrícula ao receber evento de pagamento realizado deve ativar e commitar")]
        [Trait("Categoria", "Gestão Aluno - Application - Services - PagamentoMatriculaIntegrationHandler")]
        public async Task FinalizarMatricula_EventoRecebido_DeveAtivarEComitar()
        {
            // Arrange
            CriarMatriculaTeste();
            ConfigurarAlunoRepositoryMock();
            ConfigurarUnitOfWorkMock();
            ConfigurarServiceProviderMock();

            Func<MatriculaPagamentoRealizadoIntegrationEvent, Task>? acaoFinalizar = null;

            _messageBusMock.Setup(b => b.SubscribeAsync<MatriculaPagamentoRecusadoIntegrationEvent>(
                It.IsAny<string>(), It.IsAny<Func<MatriculaPagamentoRecusadoIntegrationEvent, Task>>()));

            _messageBusMock.Setup(b => b.SubscribeAsync<MatriculaPagamentoRealizadoIntegrationEvent>(
                It.IsAny<string>(), It.IsAny<Func<MatriculaPagamentoRealizadoIntegrationEvent, Task>>()))
                .Callback<string, Func<MatriculaPagamentoRealizadoIntegrationEvent, Task>>((_, f) => acaoFinalizar = f);

            var handler = new PagamentoMatriculaIntegrationHandler(_messageBusMock.Object, _rootServiceProviderMock!.Object);

            // Act
            await handler.StartAsync(CancellationToken.None);
            await acaoFinalizar!(new MatriculaPagamentoRealizadoIntegrationEvent(_matricula!.Id));

            // Assert
            _alunoRepositoryMock!.Verify(r => r.ObterMatriculaComAlunoPorId(_matricula.Id, It.IsAny<CancellationToken>()), Times.Once);
            _uowMock!.Verify(u => u.Commit(), Times.Once);
            Assert.Equal(SituacaoMatricula.Ativa, _matricula.SituacaoMatricula);
        }

        private void CriarMatriculaTeste()
        {
            var cursoId = Guid.NewGuid();
            _matricula = new Matricula(cursoId, "Curso Teste", totalAulasCurso: 2, valor: 100m);
            var aluno = new Aluno(Guid.NewGuid(), "Aluno Teste", "a@teste.com");
            aluno.RealizarMatricula(_matricula);
            var alunoField = typeof(Matricula).GetField(
                "<Aluno>k__BackingField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            alunoField?.SetValue(_matricula, aluno);
        }

        private void ConfigurarAlunoRepositoryMock()
        {
            _alunoRepositoryMock!.Setup(r => r.ObterMatriculaComAlunoPorId(_matricula!.Id, It.IsAny<CancellationToken>()))
                                .ReturnsAsync(_matricula);
        }

        private void ConfigurarUnitOfWorkMock()
        {
            _uowMock!.Setup(u => u.Commit()).ReturnsAsync(true);
            _alunoRepositoryMock!.SetupGet(r => r.UnitOfWork).Returns(_uowMock.Object);
        }

        private void ConfigurarServiceProviderMock()
        {
            var serviceScopeMock = new Mock<IServiceScope>();
            var innerProviderMock = new Mock<IServiceProvider>();
            innerProviderMock.Setup(p => p.GetService(typeof(IAlunoRepository))).Returns(_alunoRepositoryMock!.Object);
            serviceScopeMock.Setup(s => s.ServiceProvider).Returns(innerProviderMock.Object);

            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            serviceScopeFactoryMock.Setup(f => f.CreateScope()).Returns(serviceScopeMock.Object);

            _rootServiceProviderMock!.Setup(p => p.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactoryMock.Object);
        }
    }
}