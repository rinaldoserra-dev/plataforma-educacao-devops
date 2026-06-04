using Moq;
using PlataformaEducacao.Core.Mediator;
using PlataformaEducacao.GestaoAluno.Application.Commands.GerarCertificado;
using PlataformaEducacao.GestaoAluno.Application.Events;
using PlataformaEducacao.GestaoAluno.Domain.Events;

namespace PlataformaEducacao.GestaoAluno.Application.Tests.Events
{
    public class MatriculaEventHandlerTest
    {
        [Fact(DisplayName = "MatriculaEventHandler quando CursoFinalizadoEvent recebido deve enviar GerarCertificadoCommand")]
        [Trait("Categoria", "Gestão Aluno - Application - Events - MatriculaEventHandler")]
        public async Task Handle_CursoFinalizadoEvent_EnviaGerarCertificadoCommand()
        {
            // Arrange
            var matriculaId = System.Guid.NewGuid();
            var evento = new CursoFinalizadoEvent(matriculaId);

            var mediatorMock = new Mock<IMediatorHandler>();
            mediatorMock.Setup(m => m.SendCommand(It.IsAny<GerarCertificadoCommand>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());

            var handler = new MatriculaEventHandler(mediatorMock.Object);

            // Act
            await handler.Handle(evento, CancellationToken.None);

            // Assert
            mediatorMock.Verify(m => m.SendCommand(It.Is<GerarCertificadoCommand>(c => c.MatriculaId == matriculaId)), Times.Once);
        }

        [Fact(DisplayName = "MatriculaEventHandler ao receber MatriculaAtivadaEvent não chama o mediator")]
        [Trait("Categoria", "Gestão Aluno - Application - Events - MatriculaEventHandler")]
        public async Task Handle_MatriculaAtivadaEvent_NaoChamaMediator()
        {
            // Arrange
            var evento = new MatriculaAtivadaEvent(System.Guid.NewGuid());
            var mediatorMock = new Mock<IMediatorHandler>(MockBehavior.Strict);
            var handler = new MatriculaEventHandler(mediatorMock.Object);

            // Act
            await handler.Handle(evento, CancellationToken.None);

            // Assert
            // Atualmente não há interação com o mediator.
            // Futuramente pode haver, para envio de e-mail, por exemplo.
            mediatorMock.VerifyNoOtherCalls();
        }
    }
}