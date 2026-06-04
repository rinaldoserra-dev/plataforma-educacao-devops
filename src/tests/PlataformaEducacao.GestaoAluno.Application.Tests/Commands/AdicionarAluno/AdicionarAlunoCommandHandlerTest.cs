using Moq;
using PlataformaEducacao.Core.Data;
using PlataformaEducacao.GestaoAluno.Application.Commands.AdicionarAluno;
using PlataformaEducacao.GestaoAluno.Domain;
using PlataformaEducacao.GestaoAluno.Domain.Repositories;

namespace PlataformaEducacao.GestaoAluno.Application.Tests.Commands.AdicionarAluno
{
    public class AdicionarAlunoCommandHandlerTest
    {
        [Fact(DisplayName = "Handle quando comando inválido retorna resultado de validação e não chama o repositório")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - AdicionarAlunoCommandHandler")]
        public async Task Handle_CommandInvalido_RetornaCommandValidationResultENaoChamaRepositorio()
        {
            // Arrange
            var comandoInvalido = new AdicionarAlunoCommand(Guid.Empty, "", "");
            var repositorioMock = new Mock<IAlunoRepository>(MockBehavior.Strict);
            var handler = new AdicionarAlunoCommandHandler(repositorioMock.Object);

            // Act
            var result = await handler.Handle(comandoInvalido, CancellationToken.None);

            // Assert
            Assert.Same(comandoInvalido.ValidationResult, result);
            repositorioMock.Verify(r => r.Inserir(It.IsAny<Aluno>(), It.IsAny<CancellationToken>()), Times.Never);
            repositorioMock.VerifyGet(r => r.UnitOfWork, Times.Never);
        }

        [Theory(DisplayName = "Handle quando comando válido persiste conforme resultado do Commit do UnitOfWork")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - AdicionarAlunoCommandHandler")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Handle_CommandValido_PersisteConformeResultadoCommit(bool resultadoCommit)
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var nome = "Aluno Teste";
            var email = "aluno@teste.com";
            var commandValido = new AdicionarAlunoCommand(usuarioId, nome, email);

            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(u => u.Commit()).ReturnsAsync(resultadoCommit);

            var repositorioMock = new Mock<IAlunoRepository>();
            repositorioMock.SetupGet(r => r.UnitOfWork).Returns(uowMock.Object);

            Aluno? alunoObtido = null;
            repositorioMock
                .Setup(r => r.Inserir(It.IsAny<Aluno>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Callback<Aluno, CancellationToken>((a, ct) => alunoObtido = a);

            var handler = new AdicionarAlunoCommandHandler(repositorioMock.Object);

            // Act
            var resultado = await handler.Handle(commandValido, CancellationToken.None);

            // Assert

            // Se resultadoCommit for verdadeiro, não deve haver erro de persistência; se for falso, deve haver.
            Assert.Equal(resultadoCommit, resultado.IsValid);

            if (resultadoCommit is false)
                Assert.Contains(resultado.Errors, e => e.ErrorMessage == "Houve um erro ao persistir os dados");

            // Inserir deve ter sido chamado uma vez com um aluno contendo o mesmo Id e Nome
            repositorioMock.Verify(r => r.Inserir(It.IsAny<Aluno>(), It.IsAny<CancellationToken>()), Times.Once);
            repositorioMock.VerifyGet(r => r.UnitOfWork, Times.AtLeastOnce);
            uowMock.Verify(u => u.Commit(), Times.Once);

            Assert.NotNull(alunoObtido);
            Assert.Equal(usuarioId, alunoObtido.Id);
            Assert.Equal(nome, alunoObtido.Nome);
        }

        [Fact(DisplayName = "Handle quando repositório lança exceção durante Inserir deve propagar a exceção")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - AdicionarAlunoCommandHandler")]
        public async Task Handle_QuandoRepositorioInserirLancaExcecao_DevePropagarExcecao()
        {
            // Arrange
            var command = new AdicionarAlunoCommand(Guid.NewGuid(), "Nome", "email@teste.com");
            var repositorioMock = new Mock<IAlunoRepository>();
            var uowMock = new Mock<IUnitOfWork>();
            repositorioMock.SetupGet(r => r.UnitOfWork).Returns(uowMock.Object);

            repositorioMock
                .Setup(r => r.Inserir(It.IsAny<Aluno>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("db error"));

            var handler = new AdicionarAlunoCommandHandler(repositorioMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));

            repositorioMock.Verify(r => r.Inserir(It.IsAny<Aluno>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}