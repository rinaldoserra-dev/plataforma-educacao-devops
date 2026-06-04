using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PlataformaEducacao.Core.Mediator;
using PlataformaEducacao.Core.Messages.Integration;
using PlataformaEducacao.GestaoAluno.Application.Commands.AdicionarAluno;
using PlataformaEducacao.GestaoAluno.Application.Services;
using PlataformaEducacao.MessageBus;

namespace PlataformaEducacao.GestaoAluno.Application.Tests.Services
{
    public class RegistroAlunoIntegrationHandlerTest
    {
        [Fact(DisplayName = "Registrar aluno ao responder evento deve enviar comando e retornar sucesso")]
        [Trait("Categoria", "Gestão Aluno - Application - Services - RegistroAlunoIntegrationHandler")]
        public async Task RegistrarAluno_QuandoEventoRecebido_DeveEnviarComandoERetornarSucesso()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var nome = "Aluno Novo";
            var email = "aluno@teste.com";
            var evento = new UsuarioRegistradoIntegrationEvent(usuarioId, nome, email);

            var mediatorMock = new Mock<IMediatorHandler>();
            mediatorMock.Setup(m => m.SendCommand(It.IsAny<AdicionarAlunoCommand>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            var serviceScopeMock = new Mock<IServiceScope>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(p => p.GetService(typeof(IMediatorHandler))).Returns(mediatorMock.Object);
            serviceScopeMock.Setup(s => s.ServiceProvider).Returns(serviceProviderMock.Object);

            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            serviceScopeFactoryMock.Setup(f => f.CreateScope()).Returns(serviceScopeMock.Object);

            var rootProviderMock = new Mock<IServiceProvider>();
            rootProviderMock.Setup(p => p.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactoryMock.Object);

            var busMock = new Mock<IMessageBus>();
            var advancedBusMock = new Mock<IAdvancedBus>();
            busMock.SetupGet(b => b.AdvancedBus).Returns(advancedBusMock.Object);

            Func<UsuarioRegistradoIntegrationEvent, Task<ResponseMessage>>? responderCapturado = null;
            busMock.Setup(b => b.RespondAsync<UsuarioRegistradoIntegrationEvent, ResponseMessage>(
                It.IsAny<Func<UsuarioRegistradoIntegrationEvent, Task<ResponseMessage>>>()))
                .Callback<Func<UsuarioRegistradoIntegrationEvent, Task<ResponseMessage>>>(r => responderCapturado = r)
                .Returns(Mock.Of<IDisposable>);

            var handler = new RegistroAlunoIntegrationHandler(busMock.Object, rootProviderMock.Object);

            // Act
            var tarefa = handler.StartAsync(CancellationToken.None);
            await Task.Delay(50); // pequeno atraso para permitir que o responder seja definido

            // Invocar o responder capturado e aguardar o resultado
            Assert.NotNull(responderCapturado);
            var resposta = await responderCapturado!(evento);

            // Assert
            mediatorMock.Verify(m => m.SendCommand(It.Is<AdicionarAlunoCommand>(
                c => c.UsuarioId == usuarioId && c.Nome == nome && c.Email == email)), Times.Once);
            Assert.NotNull(resposta);
            Assert.True(resposta.ValidationResult.IsValid);
        }
    }
}